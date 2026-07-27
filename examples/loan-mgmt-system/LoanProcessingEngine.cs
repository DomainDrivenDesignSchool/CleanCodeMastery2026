using ClosedXML.Excel;
using System.Text.Json;

namespace LoanManagementSystem;

public class LoanProcessingEngine
{
    private readonly string _loanApprovalExcelPath;
    private readonly string _loanCancellationExcelPath;
    private readonly string _logDirectory;
    private readonly string _deadLetterDirectory;
    private readonly string _processedApplicantsFile;
    private readonly string _auditTrailDirectory;
    private readonly string _retryTrackerFile;
    private readonly ExternalLoanService _loanService;
    private readonly int _maxRetryAttempts = 5;

    // Status reference values for loan processing
    private const int STATUS_APPROVED = 109;
    private const int STATUS_PENDING = 157;
    private const int STATUS_IN_REVIEW = 156;

    // Region identifiers
    private const int REGION_METROPOLITAN = 1;
    private const int REGION_NORTHERN = 31;

    public LoanProcessingEngine()
    {
        _loanApprovalExcelPath = Path.Combine(AppContext.BaseDirectory, "loan_requests.xlsx");
        _loanCancellationExcelPath = Path.Combine(AppContext.BaseDirectory, "loan_cancellations.xlsx");
        _loanService = new ExternalLoanService();

        // Initialize directories
        _logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
        _deadLetterDirectory = Path.Combine(AppContext.BaseDirectory, "FailedRequests");
        _auditTrailDirectory = Path.Combine(AppContext.BaseDirectory, "AuditTrails");
        _processedApplicantsFile = Path.Combine(AppContext.BaseDirectory, "ProcessedApplicants.txt");
        _retryTrackerFile = Path.Combine(AppContext.BaseDirectory, "RetryTracker.json");

        Directory.CreateDirectory(_logDirectory);
        Directory.CreateDirectory(_deadLetterDirectory);
        Directory.CreateDirectory(_auditTrailDirectory);
    }

    // ============ DATA MODELS ============

    public class LoanRequestRecord
    {
        public int Id { get; set; }
        public string ChassisNumber { get; set; }
        public string NationalId { get; set; }
        public string PolicyId { get; set; }
        public string PlateRegionCode { get; set; }
        public string PlateFirstPart { get; set; }
        public string PlateLetter { get; set; }
        public string PlateSecondPart { get; set; }
        public string FullPlateNumber { get; set; }
        public int ImportStatus { get; set; }
        public string ImportDigits { get; set; }
        public string ImportCode { get; set; }
        public string ImportCountry { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleUsage { get; set; }
        public int ContractStatus { get; set; }
        public string ContractStatusDisplay { get; set; }
        public int IsActive { get; set; }
        public int IsActiveDisplay { get; set; }
        public string RegionName { get; set; }
        public int RegionId { get; set; }
        public int BranchId { get; set; }
        public string CityName { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }
        public int RowNumber { get; set; }
        public int LoanAmount { get; set; }
        public int ProcessingMode { get; set; }
        public string ReasonCode { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationDescription { get; set; }
        public string RequestDate { get; set; }
    }

    // ============ PUBLIC ENTRY POINTS ============

    public async Task ProcessLoanApprovalRequestsAsync()
    {
        LogStep("Starting Loan Approval Request Processing");
        LogInfo($"Excel file path: {_loanApprovalExcelPath}");

        try
        {
            if (!File.Exists(_loanApprovalExcelPath))
            {
                LogError($"Excel file not found at: {_loanApprovalExcelPath}");
                return;
            }

            var allRecords = ReadExcelData(_loanApprovalExcelPath, RequestType.Approval);
            var processedApplicants = await GetProcessedApplicantsAsync();
            var retryTracker = await GetRetryTrackerAsync();

            // Filter records for processing
            var recordsToProcess = allRecords
                .Where(r => !processedApplicants.Contains(r.ChassisNumber, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (!recordsToProcess.Any())
            {
                LogWarning("All applicants have already been processed successfully.");
                return;
            }

            // Check retry limits
            var recordsWithRetryInfo = recordsToProcess
                .Select(record => new
                {
                    Record = record,
                    RetryCount = retryTracker.TryGetValue(record.ChassisNumber, out var count) ? count : 0,
                    ShouldProcess = !retryTracker.TryGetValue(record.ChassisNumber, out var retryCount) || retryCount < _maxRetryAttempts
                })
                .ToList();

            var recordsToActuallyProcess = recordsWithRetryInfo.Where(r => r.ShouldProcess).Select(r => r.Record).ToList();
            var recordsExceedingRetries = recordsWithRetryInfo.Where(r => !r.ShouldProcess).Select(r => r.Record).ToList();

            if (recordsExceedingRetries.Any())
            {
                LogWarning($"Skipping {recordsExceedingRetries.Count} applicants exceeding max retry attempts ({_maxRetryAttempts}):");
                foreach (var record in recordsExceedingRetries)
                {
                    LogWarning($"  - {record.ChassisNumber} (Attempts: {retryTracker[record.ChassisNumber]})");
                }
                var failedResults = recordsExceedingRetries.Select(r => new ProcessingResult(r.ChassisNumber, false, $"Exceeded max retry attempts ({_maxRetryAttempts})")).ToList();
                await ArchiveToDeadLetterAsync(failedResults, RequestType.Approval);
            }

            if (!recordsToActuallyProcess.Any())
            {
                LogWarning("No applicants to process (all have exceeded max retry attempts).");
                return;
            }

            LogSuccess($"Processing {recordsToActuallyProcess.Count} applicants (skipping {processedApplicants.Count} already processed, {recordsExceedingRetries.Count} exceeded retry limit)");

            var results = await ProcessRecordsAsync(recordsToActuallyProcess, RequestType.Approval);
            await UpdateRetryTrackerAsync(results.Where(r => !r.IsSuccess));
            await MarkApplicantsAsProcessedAsync(results.Where(r => r.IsSuccess).Select(r => r.ChassisNumber));
            await ArchiveToDeadLetterAsync(results.Where(r => !r.IsSuccess), RequestType.Approval);

            DisplaySummary(results, "Loan Approval");
        }
        catch (Exception ex)
        {
            LogError($"Error processing loan approval requests: {ex.Message}");
            LogDebug($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task ProcessLoanCancellationRequestsAsync()
    {
        LogStep("Starting Loan Cancellation Request Processing");
        LogInfo($"Excel file path: {_loanCancellationExcelPath}");

        try
        {
            if (!File.Exists(_loanCancellationExcelPath))
            {
                LogError($"Excel file not found at: {_loanCancellationExcelPath}");
                return;
            }

            var allRecords = ReadExcelData(_loanCancellationExcelPath, RequestType.Cancellation);
            var processedApplicants = await GetProcessedApplicantsAsync();
            var retryTracker = await GetRetryTrackerAsync();

            var recordsToProcess = allRecords
                .Where(r => !processedApplicants.Contains(r.ChassisNumber, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (!recordsToProcess.Any())
            {
                LogWarning("All applicants have already been processed successfully.");
                return;
            }

            var recordsWithRetryInfo = recordsToProcess
                .Select(record => new
                {
                    Record = record,
                    RetryCount = retryTracker.TryGetValue(record.ChassisNumber, out var count) ? count : 0,
                    ShouldProcess = !retryTracker.TryGetValue(record.ChassisNumber, out var retryCount) || retryCount < _maxRetryAttempts
                })
                .ToList();

            var recordsToActuallyProcess = recordsWithRetryInfo.Where(r => r.ShouldProcess).Select(r => r.Record).ToList();
            var recordsExceedingRetries = recordsWithRetryInfo.Where(r => !r.ShouldProcess).Select(r => r.Record).ToList();

            if (recordsExceedingRetries.Any())
            {
                LogWarning($"Skipping {recordsExceedingRetries.Count} applicants exceeding max retry attempts ({_maxRetryAttempts}):");
                foreach (var record in recordsExceedingRetries)
                {
                    LogWarning($"  - {record.ChassisNumber} (Attempts: {retryTracker[record.ChassisNumber]})");
                }
                var failedResults = recordsExceedingRetries.Select(r => new ProcessingResult(r.ChassisNumber, false, $"Exceeded max retry attempts ({_maxRetryAttempts})")).ToList();
                await ArchiveToDeadLetterAsync(failedResults, RequestType.Cancellation);
            }

            if (!recordsToActuallyProcess.Any())
            {
                LogWarning("No applicants to process (all have exceeded max retry attempts).");
                return;
            }

            LogSuccess($"Processing {recordsToActuallyProcess.Count} applicants (skipping {processedApplicants.Count} already processed, {recordsExceedingRetries.Count} exceeded retry limit)");

            var results = await ProcessRecordsAsync(recordsToActuallyProcess, RequestType.Cancellation);
            await UpdateRetryTrackerAsync(results.Where(r => !r.IsSuccess));
            await MarkApplicantsAsProcessedAsync(results.Where(r => r.IsSuccess).Select(r => r.ChassisNumber));
            await ArchiveToDeadLetterAsync(results.Where(r => !r.IsSuccess), RequestType.Cancellation);

            DisplaySummary(results, "Loan Cancellation");
        }
        catch (Exception ex)
        {
            LogError($"Error processing loan cancellation requests: {ex.Message}");
            LogDebug($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    // ============ TEST METHODS ============

    public async Task<bool> TestFirstApprovalRequestAsync()
    {
        LogStep("Testing First Request from Approval List");
        try
        {
            if (!File.Exists(_loanApprovalExcelPath))
            {
                LogError($"Excel file not found at: {_loanApprovalExcelPath}");
                return false;
            }

            var records = ReadExcelData(_loanApprovalExcelPath, RequestType.Approval);
            if (!records.Any())
            {
                LogWarning("No data found in the Approval Excel file.");
                return false;
            }

            var testRecord = records.First();
            LogInfo($"Testing first request from Approval list: {testRecord.ChassisNumber}");
            return await TestSingleRecordAsync(testRecord, RequestType.Approval);
        }
        catch (Exception ex)
        {
            LogError($"Error testing first Approval request: {ex.Message}");
            LogDebug($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<bool> TestFirstCancellationRequestAsync()
    {
        LogStep("Testing First Request from Cancellation List");
        try
        {
            if (!File.Exists(_loanCancellationExcelPath))
            {
                LogError($"Excel file not found at: {_loanCancellationExcelPath}");
                return false;
            }

            var records = ReadExcelData(_loanCancellationExcelPath, RequestType.Cancellation);
            if (!records.Any())
            {
                LogWarning("No data found in the Cancellation Excel file.");
                return false;
            }

            var testRecord = records.First();
            LogInfo($"Testing first request from Cancellation list: {testRecord.ChassisNumber}");
            return await TestSingleRecordAsync(testRecord, RequestType.Cancellation);
        }
        catch (Exception ex)
        {
            LogError($"Error testing first Cancellation request: {ex.Message}");
            LogDebug($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    // ============ PRIVATE PROCESSING METHODS ============

    private async Task<bool> TestSingleRecordAsync(LoanRequestRecord record, RequestType requestType)
    {
        LogInfo($"Testing Chassis: {record.ChassisNumber}");

        try
        {
            var processedApplicants = await GetProcessedApplicantsAsync();
            if (processedApplicants.Contains(record.ChassisNumber))
            {
                LogWarning($"Chassis {record.ChassisNumber} has already been processed successfully.");
                return true;
            }

            var result = await ProcessSingleRecordWithRetryAsync(record, requestType);

            if (result.IsSuccess)
            {
                LogSuccess($"✓ Test Chassis {record.ChassisNumber} processed successfully!");
                LogInfo($"Status: {result.Message}");

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Test successful! Process all remaining records? (Y/N): ");
                Console.ResetColor();
                var response = Console.ReadLine()?.Trim().ToUpper();

                if (response == "Y")
                {
                    if (requestType == RequestType.Approval)
                        await ProcessLoanApprovalRequestsAsync();
                    else
                        await ProcessLoanCancellationRequestsAsync();
                }
                return true;
            }
            else
            {
                LogError($"✗ Test Chassis {record.ChassisNumber} processing failed.");
                LogError($"Reason: {result.Message}");
                return false;
            }
        }
        catch (Exception ex)
        {
            LogError($"Error testing Chassis {record.ChassisNumber}: {ex.Message}");
            LogDebug($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    private async Task<List<ProcessingResult>> ProcessRecordsAsync(List<LoanRequestRecord> records, RequestType requestType)
    {
        var results = new List<ProcessingResult>();
        var total = records.Count;
        var processedCount = 0;

        foreach (var record in records)
        {
            processedCount++;
            LogProgress($"Processing Chassis {processedCount}/{total}: {record.ChassisNumber}");

            try
            {
                var result = await ProcessSingleRecordWithRetryAsync(record, requestType);
                results.Add(result);

                if (result.IsSuccess)
                {
                    LogSuccess($"✓ Chassis {record.ChassisNumber} processed successfully ({processedCount}/{total})");
                }
                else
                {
                    LogError($"✗ Chassis {record.ChassisNumber} processing failed after {_maxRetryAttempts} attempts: {result.Message} ({processedCount}/{total})");
                }
            }
            catch (Exception ex)
            {
                LogError($"✗ Chassis {record.ChassisNumber} threw exception: {ex.Message}");
                results.Add(new ProcessingResult(record.ChassisNumber, false, $"Exception: {ex.Message}"));

                await LogAuditTrailAsync(record.ChassisNumber, requestType, null, null, null,
                    new { Exception = ex.Message, ex.StackTrace }, _maxRetryAttempts, false);
            }

            UpdateProgress(processedCount, total);
        }

        return results;
    }

    private async Task<ProcessingResult> ProcessSingleRecordWithRetryAsync(LoanRequestRecord record, RequestType requestType)
    {
        int attempt = 0;
        ProcessingResult lastResult = null;
        object lastRequest = null;
        object lastResponse = null;

        while (attempt < _maxRetryAttempts)
        {
            attempt++;
            try
            {
                LogDebug($"Attempt {attempt}/{_maxRetryAttempts} for Chassis: {record.ChassisNumber}");

                var (result, request, response) = await ProcessSingleRecordWithDetailsAsync(record, requestType);

                lastRequest = request;
                lastResponse = response;
                lastResult = result;

                await LogAuditTrailAsync(record.ChassisNumber, requestType, request, response, result, null, attempt, result.IsSuccess);

                if (result.IsSuccess)
                {
                    return result;
                }

                if (attempt < _maxRetryAttempts)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    LogWarning($"Retry {attempt}/{_maxRetryAttempts} failed for Chassis {record.ChassisNumber}. Waiting {delay.TotalSeconds}s before retry...");
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                lastResult = new ProcessingResult(record.ChassisNumber, false, $"Attempt {attempt}: {ex.Message}");
                lastRequest = null;
                lastResponse = null;

                await LogAuditTrailAsync(record.ChassisNumber, requestType, null, null, lastResult,
                    new { Exception = ex.Message, ex.StackTrace }, attempt, false);

                if (attempt < _maxRetryAttempts)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    LogWarning($"Exception on attempt {attempt} for Chassis {record.ChassisNumber}. Waiting {delay.TotalSeconds}s before retry...");
                    await Task.Delay(delay);
                }
            }
        }

        await LogAuditTrailAsync(record.ChassisNumber, requestType, lastRequest, lastResponse, lastResult,
            new { FinalStatus = "All retry attempts failed" }, _maxRetryAttempts, false);

        return lastResult ?? new ProcessingResult(record.ChassisNumber, false, "All retry attempts failed");
    }

    private async Task<(ProcessingResult Result, object Request, object Response)> ProcessSingleRecordWithDetailsAsync(
        LoanRequestRecord record, RequestType requestType)
    {
        try
        {
            var request = BuildRequestFromExcel(record, requestType);
            if (request == null)
            {
                return (new ProcessingResult(record.ChassisNumber, false, "Failed to build request DTO"), null, null);
            }

            var operationMode = requestType == RequestType.Approval ? 1 : 2;
            var response = await Task.Run(() => _loanService.ExecuteLoanOperation(request, operationMode));

            var result = ProcessServiceResponse(record, response, requestType);
            return (result, request, response);
        }
        catch (Exception ex)
        {
            LogError($"Error processing Chassis {record.ChassisNumber}: {ex.Message}");
            return (new ProcessingResult(record.ChassisNumber, false, $"Error: {ex.Message}"), null, null);
        }
    }

    private ExternalLoanRequest BuildRequestFromExcel(LoanRequestRecord record, RequestType requestType)
    {
        try
        {
            var request = new ExternalLoanRequest();
            var isSpecialRegion = record.RegionId == REGION_METROPOLITAN || record.RegionId == REGION_NORTHERN;

            if (requestType == RequestType.Approval)
            {
                var approvalRequest = new LoanApprovalRequest
                {
                    branchIds = null,
                    description = record.Description ?? "Standard loan approval request",
                    branchCode = record.BranchId.ToString("D2"),
                    nationalId = record.NationalId,
                    reason = record.ReasonCode ?? "01",
                    requestDate = DateTime.Now,
                    applicantId = record.NationalId?.Trim(),
                    organizationId = record.OrganizationId,
                    organizationDescription = record.OrganizationDescription ?? "General applicant",
                    phoneNumber = null,
                    contractNumber = null,
                    fromDate = record.ValidFromDate,
                    toDate = record.ValidToDate,
                    manufactureYear = null,
                    policyId = record.PolicyId,
                    chassisNumber = record.ChassisNumber?.Trim(),
                    vehicleType = 0,
                    processingMode = record.ProcessingMode,
                    loanAmount = record.LoanAmount
                };

                if (isSpecialRegion)
                {
                    approvalRequest.regionId = "32";
                }
                else
                {
                    approvalRequest.regionId = record.RegionId.ToString("D2");
                }

                if (record.ImportStatus == 1)
                {
                    approvalRequest.importPlateDigits = record.ImportDigits?.ToString();
                }
                else
                {
                    approvalRequest.plateNumber = record.FullPlateNumber;
                }

                request.ApprovalRequest = approvalRequest;
                return request;
            }
            else
            {
                var cancellationRequest = new LoanCancellationRequest
                {
                    plateNumber = record.ImportStatus == 1 ? null : record.FullPlateNumber,
                    importPlateDigits = record.ImportStatus == 1 ? record.ImportDigits?.ToString() : null,
                    description = record.Description ?? "Loan cancellation request",
                    reason = record.ReasonCode ?? "01",
                    requestDate = DateTime.Now,
                    applicantId = record.NationalId?.Trim(),
                    organizationId = record.OrganizationId,
                    organizationDescription = record.OrganizationDescription ?? "General applicant",
                    phoneNumber = null,
                    contractNumber = null,
                    fromDate = DateTime.Now,
                    toDate = DateTime.Now.AddYears(1),
                    manufactureYear = null,
                    policyId = record.PolicyId,
                    chassisNumber = record.ChassisNumber?.Trim(),
                    vehicleType = 0,
                    loanAmount = record.LoanAmount
                };

                request.CancellationRequest = cancellationRequest;
                return request;
            }
        }
        catch (Exception ex)
        {
            LogError($"Error building request from Excel: {ex.Message}");
            return null;
        }
    }

    private ProcessingResult ProcessServiceResponse(LoanRequestRecord record, object response, RequestType requestType)
    {
        try
        {
            if (response != null)
            {
                return new ProcessingResult(record.ChassisNumber, true, "Success");
            }
            return new ProcessingResult(record.ChassisNumber, false, "Response processing not implemented");
        }
        catch (Exception ex)
        {
            LogError($"Error processing response: {ex.Message}");
            return new ProcessingResult(record.ChassisNumber, false, $"Response error: {ex.Message}");
        }
    }

    // ============ EXCEL READING ============

    private List<LoanRequestRecord> ReadExcelData(string filePath, RequestType requestType)
    {
        try
        {
            LogDebug($"Reading data from Excel: {filePath}");

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);

            var headerRow = worksheet.Row(1);
            var columnMap = new Dictionary<string, int>();

            for (int col = 1; col <= headerRow.LastCellUsed().Address.ColumnNumber; col++)
            {
                var headerValue = headerRow.Cell(col).GetString().Trim();
                columnMap[headerValue] = col;
            }

            var records = new List<LoanRequestRecord>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                try
                {
                    var record = new LoanRequestRecord
                    {
                        Id = row.GetInt(columnMap, "id"),
                        ChassisNumber = row.GetString(columnMap, "ChassisNumber"),
                        NationalId = row.GetString(columnMap, "NationalId"),
                        PolicyId = row.GetString(columnMap, "policyId"),
                        PlateRegionCode = row.GetString(columnMap, "PlateRegionCode"),
                        PlateFirstPart = row.GetString(columnMap, "PlateFirstPart"),
                        PlateLetter = row.GetString(columnMap, "PlateLetter"),
                        PlateSecondPart = row.GetString(columnMap, "PlateSecondPart"),
                        ImportStatus = row.GetInt(columnMap, "ImportStatus"),
                        ImportDigits = row.GetString(columnMap, "ImportDigits", null),
                        ImportCode = row.GetString(columnMap, "ImportCode", null),
                        ImportCountry = row.GetString(columnMap, "ImportCountry", null),
                        RegionId = row.GetInt(columnMap, "RegionId"),
                        BranchId = row.GetInt(columnMap, "BranchId"),
                        OrganizationId = row.GetInt(columnMap, "organizationId"),
                        OrganizationDescription = row.GetString(columnMap, "organizationDescription", "General applicant"),
                        ReasonCode = row.GetString(columnMap, "reason", "01"),
                        Description = row.GetString(columnMap, "description", null),
                        RequestDate = row.GetString(columnMap, "requestDate", null),
                        FullPlateNumber = "",
                        LoanAmount = row.GetInt(columnMap, "loanAmount", 0)
                    };

                    record.FullPlateNumber = $"{record.PlateRegionCode}{record.PlateFirstPart}{record.PlateLetter}{record.PlateSecondPart}";

                    if (requestType == RequestType.Approval)
                    {
                        record.VehicleCategory = row.GetString(columnMap, "VehicleCategory");
                        record.VehicleUsage = row.GetString(columnMap, "VehicleUsage");
                        record.ContractStatus = row.GetInt(columnMap, "contractStatus");
                        record.ContractStatusDisplay = row.GetString(columnMap, "ContractStatusDisplay");
                        record.IsActive = row.GetInt(columnMap, "IsActive");
                        record.RegionName = row.GetString(columnMap, "RegionName");
                        record.CityName = row.GetString(columnMap, "CityName");
                        record.IsActiveDisplay = row.GetInt(columnMap, "IsActiveDisplay");
                        record.ValidFromDate = row.GetDateTime(columnMap, "validFrom", DateTime.Now);
                        record.ValidToDate = row.GetDateTime(columnMap, "validTo", DateTime.Now.AddYears(1));
                        record.RowNumber = row.GetInt(columnMap, "rowNumber");
                        record.LoanAmount = row.GetInt(columnMap, "loanAmount", 0);
                        record.ProcessingMode = row.GetInt(columnMap, "processingMode");
                    }
                    else
                    {
                        var loanAmountStr = row.GetString(columnMap, "loanAmount");
                        record.LoanAmount = int.TryParse(loanAmountStr, out var val) ? val : 0;
                        record.ProcessingMode = 1;
                    }

                    records.Add(record);
                }
                catch (Exception ex)
                {
                    LogWarning($"Error reading row in Excel: {ex.Message}");
                }
            }

            LogDebug($"Read {records.Count} records from Excel");
            return records;
        }
        catch (Exception ex)
        {
            LogError($"Error reading Excel file: {ex.Message}");
            throw;
        }
    }

    // ============ TRACKING METHODS ============

    private async Task<Dictionary<string, int>> GetRetryTrackerAsync()
    {
        if (!File.Exists(_retryTrackerFile))
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var json = await File.ReadAllTextAsync(_retryTrackerFile);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json)
                ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private async Task UpdateRetryTrackerAsync(IEnumerable<ProcessingResult> failedResults)
    {
        if (!failedResults.Any()) return;

        var tracker = await GetRetryTrackerAsync();

        foreach (var result in failedResults)
        {
            if (tracker.ContainsKey(result.ChassisNumber))
                tracker[result.ChassisNumber]++;
            else
                tracker[result.ChassisNumber] = 1;
        }

        var json = JsonSerializer.Serialize(tracker, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_retryTrackerFile, json);

        LogInfo($"Updated retry tracker for {failedResults.Count()} failed applicants");
    }

    private async Task<HashSet<string>> GetProcessedApplicantsAsync()
    {
        if (!File.Exists(_processedApplicantsFile))
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var lines = await File.ReadAllLinesAsync(_processedApplicantsFile);
        return new HashSet<string>(lines.Where(l => !string.IsNullOrWhiteSpace(l)), StringComparer.OrdinalIgnoreCase);
    }

    private async Task MarkApplicantsAsProcessedAsync(IEnumerable<string> chassisNumbers)
    {
        if (!chassisNumbers.Any()) return;

        await File.AppendAllLinesAsync(_processedApplicantsFile, chassisNumbers);
        LogSuccess($"Marked {chassisNumbers.Count()} applicants as processed");
    }

    private async Task ArchiveToDeadLetterAsync(IEnumerable<ProcessingResult> failedResults, RequestType requestType)
    {
        if (!failedResults.Any()) return;

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"DeadLetter_{requestType}_{timestamp}.json";
        var filePath = Path.Combine(_deadLetterDirectory, fileName);

        var retryTracker = await GetRetryTrackerAsync();

        var deadLetterData = new
        {
            Timestamp = DateTime.Now,
            ProcessType = requestType.ToString(),
            TotalFailed = failedResults.Count(),
            FailedItems = failedResults.Select(r => new
            {
                r.ChassisNumber,
                r.Message,
                r.IsSuccess,
                RetryCount = retryTracker.TryGetValue(r.ChassisNumber, out var count) ? count : 0,
                MaxRetryAttempts = _maxRetryAttempts
            }).ToList()
        };

        var json = JsonSerializer.Serialize(deadLetterData, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);

        LogWarning($"Archived {failedResults.Count()} failed requests to dead letter: {filePath}");
    }

    private async Task LogAuditTrailAsync(
        string chassisNumber,
        RequestType requestType,
        object request,
        object response,
        ProcessingResult result,
        object exceptionInfo,
        int attempt,
        bool isSuccess)
    {
        try
        {
            var logFileName = $"{chassisNumber}_{requestType}_{DateTime.Now:yyyyMMdd}.json";
            var logFilePath = Path.Combine(_auditTrailDirectory, logFileName);

            var logEntry = new
            {
                Timestamp = DateTime.Now,
                ChassisNumber = chassisNumber,
                RequestType = requestType.ToString(),
                Attempt = attempt,
                MaxAttempts = _maxRetryAttempts,
                IsSuccess = isSuccess,
                Result = result != null ? new
                {
                    result.ChassisNumber,
                    result.IsSuccess,
                    result.Message
                } : null,
                Request = request,
                Response = response,
                Exception = exceptionInfo,
                Environment = new
                {
                    Environment.MachineName,
                    OSVersion = Environment.OSVersion.ToString(),
                    Environment.ProcessId
                }
            };

            var json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            if (File.Exists(logFilePath))
            {
                var existingContent = await File.ReadAllTextAsync(logFilePath);
                var entries = new List<object>();

                try
                {
                    var lines = existingContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                var entry = JsonSerializer.Deserialize<object>(line);
                                if (entry != null)
                                    entries.Add(entry);
                            }
                            catch { /* Skip invalid entries */ }
                        }
                    }
                }
                catch { /* If file is corrupted, start fresh */ }

                entries.Add(logEntry);

                var allEntriesJson = string.Join("\n", entries.Select(e => JsonSerializer.Serialize(e, new JsonSerializerOptions { WriteIndented = false })));
                await File.WriteAllTextAsync(logFilePath, allEntriesJson);
            }
            else
            {
                await File.WriteAllTextAsync(logFilePath, JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true }));
            }

            LogDebug($"Audit trail saved for Chassis {chassisNumber} (Attempt {attempt}): {logFilePath}");
        }
        catch (Exception ex)
        {
            LogError($"Failed to log audit trail for Chassis {chassisNumber}: {ex.Message}");
        }
    }

    private void DisplaySummary(List<ProcessingResult> results, string processName)
    {
        LogStep($"{processName} Process Complete!");

        var successCount = results.Count(r => r.IsSuccess);
        var failureCount = results.Count - successCount;
        var successRate = results.Any() ? successCount / (double)results.Count * 100 : 0;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine($"│                    {processName} Summary                     │");
        Console.WriteLine("├─────────────────────────────────────────────────────────┤");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"│ Total Applicants Processed: {results.Count,-30}│");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"│ Successful:               {successCount,-30}│");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"│ Failed:                   {failureCount,-30}│");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"│ Success Rate:             {successRate:F2}%{"".PadLeft(25)}│");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");
        Console.ResetColor();

        if (failureCount > 0)
        {
            LogWarning("Failed applicants (archived to Dead Letter):");
            foreach (var failed in results.Where(r => !r.IsSuccess))
            {
                LogError($"  - {failed.ChassisNumber}: {failed.Message}");
            }
        }

        Console.WriteLine();
        LogInfo($"📁 Audit trail logs saved in: {_auditTrailDirectory}");
        LogInfo($"📁 Failed requests archived in: {_deadLetterDirectory}");
        LogInfo($"📁 Processed applicants tracked in: {_processedApplicantsFile}");
        LogInfo($"📁 Retry tracker saved in: {_retryTrackerFile}");
    }

    private void UpdateProgress(int current, int total)
    {
        var barLength = 50;
        var completed = (int)(current / (double)total * barLength);
        var remaining = barLength - completed;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("\r[");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('█', completed));

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', remaining));

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"] {current}/{total} ({current / (double)total:P0})");
        Console.ResetColor();
    }

    // Logging Helper Methods
    private void LogInfo(string message) => ConsoleLogger.LogInfo(message);
    private void LogSuccess(string message) => ConsoleLogger.LogSuccess(message);
    private void LogWarning(string message) => ConsoleLogger.LogWarning(message);
    private void LogError(string message) => ConsoleLogger.LogError(message);
    private void LogDebug(string message) => ConsoleLogger.LogDebug(message);
    private void LogStep(string message) => ConsoleLogger.LogStep(message);
    private void LogProgress(string message) => ConsoleLogger.LogProgress(message);
}

public enum RequestType
{
    Approval,
    Cancellation
}

public class ProcessingResult
{
    public string ChassisNumber { get; }
    public bool IsSuccess { get; }
    public string Message { get; }

    public ProcessingResult(string chassisNumber, bool isSuccess, string message)
    {
        ChassisNumber = chassisNumber;
        IsSuccess = isSuccess;
        Message = message;
    }
}
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace LoanManagementSystem;

/// <summary>
/// Service for interacting with the external loan processing API
/// Handles all loan-related operations
/// </summary>
public class ExternalLoanService
{
    #region Constants

    private const string BASE_URL = @"http://api.internal.loan.local/api/services/LoanProcessing/";
    private const string AUTH_TOKEN = "Basic dXNlcl9sb2FuOmJMb2NrfG5HXzE0MDMwNDAz";
    private const string API_KEY = "L98m64KHjl98G";
    private const int TIMEOUT_SECONDS = 60;

    #endregion

    #region Constructor

    public ExternalLoanService()
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Main loan operation router
    /// </summary>
    public ExternalLoanResponse ExecuteLoanOperation(ExternalLoanRequest request, int operationType)
    {
        var finalResponse = new ExternalLoanResponse();

        try
        {
            switch (operationType)
            {
                case 1: // Approve/Add Loan
                    return ProcessLoanApproval(request, finalResponse);

                case 2: // Cancel Loan
                    return ProcessLoanCancellation(request, finalResponse);

                case 3: // Get Loan Status
                    return ProcessLoanStatusQuery(request, finalResponse);

                case 4: // Get Loan Report
                    return ProcessLoanReport(request, finalResponse);

                default:
                    return finalResponse;
            }
        }
        catch (Exception ex)
        {
            ConsoleLogger.LogError($"Loan operation error (Type: {operationType}): {ex.Message}");
            ConsoleLogger.LogDebug($"Stack trace: {ex.StackTrace}");

            finalResponse.IsSuccessful = false;
            return finalResponse;
        }
    }

    /// <summary>
    /// Adds a new loan record
    /// </summary>
    public ServiceResponse<LoanOperationResult> AddLoan(LoanApprovalRequest request)
    {
        var client = CreateClient("AddLoan");
        var restRequest = CreateRequest(Method.POST);

        if (request.requestDate.HasValue)
        {
            request.requestDate = new DateTime(
                request.requestDate.Value.Year,
                request.requestDate.Value.Month,
                request.requestDate.Value.Day,
                request.requestDate.Value.Hour,
                request.requestDate.Value.Minute,
                request.requestDate.Value.Second
            );
        }

        if (!string.IsNullOrEmpty(request.phoneNumber))
        {
            request.phoneNumber = "0" + request.phoneNumber;
        }

        restRequest.AddParameter("application/json", JsonConvert.SerializeObject(request), ParameterType.RequestBody);
        return ExecuteRequest<LoanOperationResult>(client, restRequest, "AddLoan");
    }

    /// <summary>
    /// Removes an existing loan record
    /// </summary>
    public ServiceResponse<LoanOperationResult> RemoveLoan(LoanCancellationRequest request)
    {
        var client = CreateClient("RemoveLoan");
        var restRequest = CreateRequest(Method.POST);

        if (!string.IsNullOrEmpty(request?.phoneNumber))
        {
            request.phoneNumber = "0" + request.phoneNumber;
        }

        restRequest.AddParameter("application/json", JsonConvert.SerializeObject(request), ParameterType.RequestBody);
        return ExecuteRequest<LoanOperationResult>(client, restRequest, "RemoveLoan");
    }

    /// <summary>
    /// Cancels an existing loan record
    /// </summary>
    public ServiceResponse<LoanOperationResult> CancelLoan(LoanCancellationRequest request)
    {
        var client = CreateClient("CancelLoan");
        var restRequest = CreateRequest(Method.POST);

        if (!string.IsNullOrEmpty(request?.phoneNumber))
        {
            request.phoneNumber = "0" + request.phoneNumber;
        }

        restRequest.AddParameter("application/json", JsonConvert.SerializeObject(request), ParameterType.RequestBody);
        return ExecuteRequest<LoanOperationResult>(client, restRequest, "CancelLoan");
    }

    /// <summary>
    /// Approves a loan application
    /// </summary>
    public ServiceResponse<LoanOperationResult> ApproveLoan(LoanApprovalRequest request)
    {
        var client = CreateClient("ApproveLoan");
        var restRequest = CreateRequest(Method.POST);

        if (!string.IsNullOrEmpty(request?.phoneNumber))
        {
            request.phoneNumber = "0" + request.phoneNumber;
        }

        restRequest.AddParameter("application/json", JsonConvert.SerializeObject(request), ParameterType.RequestBody);
        return ExecuteRequest<LoanOperationResult>(client, restRequest, "ApproveLoan");
    }

    /// <summary>
    /// Retrieves loan information for a specific applicant
    /// </summary>
    public ServiceResponse<LoanStatusDto> GetLoanStatus(string applicantId)
    {
        var client = new RestClient($"{BASE_URL}GetLoanStatus?ApplicantId={applicantId}&key={API_KEY}");
        client.Timeout = TIMEOUT_SECONDS * 1000;

        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", AUTH_TOKEN);
        request.AddHeader("Content-Type", "application/json");

        return ExecuteRequest<LoanStatusDto>(client, request, "GetLoanStatus");
    }

    /// <summary>
    /// Retrieves loan report for a specific applicant
    /// </summary>
    public ServiceResponse<LoanReportDto> GetLoanReport(string applicantId)
    {
        var client = new RestClient($"{BASE_URL}GetLoanReport?ApplicantId={applicantId}&key={API_KEY}");
        client.Timeout = TIMEOUT_SECONDS * 1000;

        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", AUTH_TOKEN);
        request.AddHeader("Content-Type", "application/json");

        return ExecuteRequest<LoanReportDto>(client, request, "GetLoanReport");
    }

    #endregion

    #region Private Helper Methods

    private RestClient CreateClient(string endpoint)
    {
        var client = new RestClient($"{BASE_URL}{endpoint}?key={API_KEY}");
        client.Timeout = TIMEOUT_SECONDS * 1000;
        return client;
    }

    private RestRequest CreateRequest(Method method)
    {
        var request = new RestRequest(method);
        request.AddHeader("Authorization", AUTH_TOKEN);
        request.AddHeader("Content-Type", "application/json");
        return request;
    }

    private ServiceResponse<T> ExecuteRequest<T>(RestClient client, RestRequest request, string operationName)
    {
        try
        {
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                return ServiceResponse<T>.FromError(
                    response.StatusCode,
                    $"Connection error to {operationName} service: {response.ErrorMessage}",
                    response.ErrorException
                );
            }

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                return ServiceResponse<T>.FromError(
                    response.StatusCode,
                    $"Error in {operationName} service: {response.ErrorMessage}",
                    null
                );
            }

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return ServiceResponse<T>.FromError(
                    response.StatusCode,
                    $"Empty response from {operationName} service",
                    null
                );
            }

            try
            {
                var data = JsonConvert.DeserializeObject<T>(response.Content);
                return ServiceResponse<T>.FromSuccess((int)response.StatusCode, data);
            }
            catch (JsonException ex)
            {
                return ServiceResponse<T>.FromError(
                    response.StatusCode,
                    $"Error parsing response from {operationName} service: {ex.Message}",
                    ex
                );
            }
        }
        catch (Exception ex)
        {
            return ServiceResponse<T>.FromError(
                HttpStatusCode.ServiceUnavailable,
                $"Unexpected error in {operationName} service: {ex.Message}",
                ex
            );
        }
    }

    #endregion

    #region Private Processing Methods

    private ExternalLoanResponse ProcessLoanApproval(ExternalLoanRequest request, ExternalLoanResponse finalResponse)
    {
        // Normalize region ID for special regions
        if (request.ApprovalRequest.regionId == "01" ||
            request.ApprovalRequest.regionId == "1" ||
            request.ApprovalRequest.regionId == "31")
        {
            request.ApprovalRequest.regionId = "32";
        }

        var result = ApproveLoan(request.ApprovalRequest);
        if (!result.IsSuccessful)
        {
            throw new Exception($"Loan approval service unavailable. Error: {result.ErrorMessage}");
        }

        finalResponse.ApprovalResult = new LoanApprovalResult
        {
            responseMessage = result.Data?.responseMessage,
            responseCode = result.Data?.responseCode,
            applicantId = result.Data?.applicantId,
            StatusCode = result.StatusCode
        };
        return finalResponse;
    }

    private ExternalLoanResponse ProcessLoanCancellation(ExternalLoanRequest request, ExternalLoanResponse finalResponse)
    {
        var result = CancelLoan(request.CancellationRequest);
        if (!result.IsSuccessful)
        {
            throw new Exception($"Loan cancellation service unavailable. Error: {result.ErrorMessage}");
        }

        finalResponse.CancellationResult = new LoanCancellationResult
        {
            responseMessage = result.Data?.responseMessage,
            responseCode = result.Data?.responseCode,
            applicantId = result.Data?.applicantId,
            StatusCode = result.StatusCode
        };
        return finalResponse;
    }

    private ExternalLoanResponse ProcessLoanStatusQuery(ExternalLoanRequest request, ExternalLoanResponse finalResponse)
    {
        var result = GetLoanStatus(request.StatusQueryInput.ApplicantId);
        if (!result.IsSuccessful)
        {
            throw new Exception($"Loan status query service unavailable. Error: {result.ErrorMessage}");
        }

        finalResponse.StatusQueryResult = new LoanStatusQueryResult
        {
            approvedBranches = new List<LoanBranchInfo>(),
            responseCode = result.Data?.responseCode,
            responseMessage = result.Data?.responseMessage,
            applicantId = result.Data?.applicantId,
            StatusCode = result.StatusCode
        };

        if (result.Data?.approvedBranches != null)
        {
            result.Data.approvedBranches.ForEach(x =>
                finalResponse.StatusQueryResult.approvedBranches.Add(new LoanBranchInfo
                {
                    branchId = x.branchId,
                    branchName = x.branchName,
                    regionId = x.regionId,
                    regionName = x.regionName
                })
            );
        }

        return finalResponse;
    }

    private ExternalLoanResponse ProcessLoanReport(ExternalLoanRequest request, ExternalLoanResponse finalResponse)
    {
        var result = GetLoanReport(request.ReportInput.ApplicantId);
        if (!result.IsSuccessful)
        {
            throw new Exception($"Loan report service unavailable. Error: {result.ErrorMessage}");
        }

        finalResponse.ReportResult = new LoanReportResult
        {
            approved = result.Data?.approved,
            amount = result.Data?.amount,
            applicantId = result.Data?.applicantId,
            description = result.Data?.description,
            StatusCode = result.StatusCode,
            approvedBranches = new List<LoanReportBranchInfo>()
        };

        if (result.Data?.approvedBranches != null)
        {
            result.Data.approvedBranches.ForEach(x =>
                finalResponse.ReportResult.approvedBranches.Add(new LoanReportBranchInfo
                {
                    regionId = x.regionId,
                    regionName = x.regionName
                })
            );
        }

        return finalResponse;
    }

    #endregion
}

/// <summary>
/// Generic service response wrapper
/// </summary>
public class ServiceResponse<T>
{
    public bool IsSuccessful { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
    public Exception ErrorException { get; set; }

    public static ServiceResponse<T> FromSuccess(int statusCode, T data)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = true,
            StatusCode = statusCode,
            Data = data,
            ErrorMessage = null,
            ErrorException = null
        };
    }

    public static ServiceResponse<T> FromError(int statusCode, string errorMessage, Exception exception = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = false,
            StatusCode = statusCode,
            Data = default,
            ErrorMessage = errorMessage,
            ErrorException = exception
        };
    }

    public static ServiceResponse<T> FromError(HttpStatusCode statusCode, string errorMessage, Exception exception = null)
    {
        return FromError((int)statusCode, errorMessage, exception);
    }
}

#region DTO Classes

public class LoanStatusDto
{
    public int StatusCode { get; set; }
    public int? responseCode { get; set; }
    public string applicantId { get; set; }
    public string responseMessage { get; set; }
    public List<LoanBranchItem> approvedBranches { get; set; }
}

public class LoanBranchItem
{
    public string branchId { get; set; }
    public string branchName { get; set; }
    public string regionId { get; set; }
    public string regionName { get; set; }
}

public class LoanApprovalRequest
{
    public List<int>? branchIds { get; set; }
    public string? applicantId { get; set; }
    public string? regionId { get; set; }
    public string? branchCode { get; set; }
    public string? phoneNumber { get; set; }
    public string? nationalId { get; set; }
    public string? plateNumber { get; set; }
    public string? importPlateDigits { get; set; }
    public string? reason { get; set; }
    public string? description { get; set; }
    public DateTime? requestDate { get; set; }
    public DateTime? fromDate { get; set; }
    public DateTime? toDate { get; set; }
    public int? organizationId { get; set; }
    public string? organizationDescription { get; set; }
    public string? contractNumber { get; set; }
    public int? manufactureYear { get; set; }
    public int? processingMode { get; set; }
    public int? vehicleType { get; set; }
    public string? policyId { get; set; }
    public string? chassisNumber { get; set; }
    public int? loanAmount { get; set; }
}

public class LoanCancellationRequest
{
    public string? applicantId { get; set; }
    public string? reason { get; set; }
    public string? description { get; set; }
    public DateTime? requestDate { get; set; }
    public string? plateNumber { get; set; }
    public string? importPlateDigits { get; set; }
    public int? organizationId { get; set; }
    public string? organizationDescription { get; set; }
    public string? phoneNumber { get; set; }
    public string? contractNumber { get; set; }
    public int? manufactureYear { get; set; }
    public int? vehicleType { get; set; }
    public string? policyId { get; set; }
    public string? chassisNumber { get; set; }
    public DateTime? fromDate { get; set; }
    public DateTime? toDate { get; set; }
    public int? loanAmount { get; set; }
}

public class LoanOperationResult
{
    public int StatusCode { get; set; }
    public int? responseCode { get; set; }
    public string? applicantId { get; set; }
    public string? responseMessage { get; set; }
}

public class LoanReportDto
{
    public int StatusCode { get; set; }
    public string applicantId { get; set; }
    public bool? approved { get; set; }
    public int? amount { get; set; }
    public string description { get; set; }
    public List<LoanReportLocationDto> approvedBranches { get; set; }
}

public class LoanReportLocationDto
{
    public string regionName { get; set; }
    public int? regionId { get; set; }
}

public class ExternalLoanRequest
{
    public LoanApprovalRequest ApprovalRequest { get; set; }
    public LoanCancellationRequest CancellationRequest { get; set; }
    public LoanStatusQueryInput StatusQueryInput { get; set; }
    public LoanReportInput ReportInput { get; set; }
}

public class LoanStatusQueryInput
{
    public string ApplicantId { get; set; }
}

public class LoanReportInput
{
    public string ApplicantId { get; set; }
}

public class ExternalLoanResponse
{
    public bool? IsSuccessful { get; set; }
    public LoanApprovalResult ApprovalResult { get; set; }
    public LoanCancellationResult CancellationResult { get; set; }
    public LoanStatusQueryResult StatusQueryResult { get; set; }
    public LoanReportResult ReportResult { get; set; }
}

public class LoanApprovalResult
{
    public int StatusCode { get; set; }
    public int? responseCode { get; set; }
    public string? applicantId { get; set; }
    public string? responseMessage { get; set; }
}

public class LoanCancellationResult
{
    public int StatusCode { get; set; }
    public int? responseCode { get; set; }
    public string? applicantId { get; set; }
    public string? responseMessage { get; set; }
}

public class LoanStatusQueryResult
{
    public int StatusCode { get; set; }
    public int? responseCode { get; set; }
    public string applicantId { get; set; }
    public string responseMessage { get; set; }
    public List<LoanBranchInfo> approvedBranches { get; set; }
}

public class LoanBranchInfo
{
    public string branchId { get; set; }
    public string branchName { get; set; }
    public string regionId { get; set; }
    public string regionName { get; set; }
}

public class LoanReportResult
{
    public int StatusCode { get; set; }
    public string applicantId { get; set; }
    public bool? approved { get; set; }
    public int? amount { get; set; }
    public string description { get; set; }
    public List<LoanReportBranchInfo> approvedBranches { get; set; }
}

public class LoanReportBranchInfo
{
    public string regionName { get; set; }
    public int? regionId { get; set; }
}

#endregion
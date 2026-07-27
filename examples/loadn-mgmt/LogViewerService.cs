using LoanManagementSystem;
using System.Text.Json;

namespace LoanManagement.Services
{
    public class LogViewerService
    {
        private readonly string _auditTrailDirectory;
        private readonly string _deadLetterDirectory;

        public LogViewerService()
        {
            _auditTrailDirectory = Path.Combine(AppContext.BaseDirectory, "AuditTrails");
            _deadLetterDirectory = Path.Combine(AppContext.BaseDirectory, "FailedRequests");
        }

        public async Task ViewAllLogsAsync()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    LOG VIEWER SERVICE                      ║");
            Console.WriteLine("║                  Loan Management System                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
                Console.WriteLine("│                    LOG VIEWER MENU                      │");
                Console.WriteLine("├─────────────────────────────────────────────────────────┤");
                Console.WriteLine("│  1. View All Audit Trail Logs                          │");
                Console.WriteLine("│  2. Search Audit Trail Logs by Chassis Number          │");
                Console.WriteLine("│  3. View All Failed Requests (Dead Letters)            │");
                Console.WriteLine("│  4. Search Failed Requests by Chassis Number           │");
                Console.WriteLine("│  5. View Summary Statistics                            │");
                Console.WriteLine("│  6. Return to Main Menu                                │");
                Console.WriteLine("└─────────────────────────────────────────────────────────┘");
                Console.ResetColor();
                Console.Write("Enter your choice: ");

                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    LogError("Invalid input. Please try again.");
                    continue;
                }

                if (int.TryParse(input, out var choice))
                {
                    switch (choice)
                    {
                        case 1:
                            await ViewAllAuditLogsAsync();
                            break;
                        case 2:
                            await SearchAuditLogsByChassisAsync();
                            break;
                        case 3:
                            await ViewAllFailedRequestsAsync();
                            break;
                        case 4:
                            await SearchFailedRequestsByChassisAsync();
                            break;
                        case 5:
                            await ViewStatisticsAsync();
                            break;
                        case 6:
                            return;
                        default:
                            LogError("Invalid choice. Please enter a number between 1 and 6.");
                            break;
                    }
                }
                else
                {
                    LogError("Invalid input. Please enter a valid number.");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private async Task ViewAllAuditLogsAsync()
        {
            Console.Clear();
            LogStep("Audit Trail Logs");

            if (!Directory.Exists(_auditTrailDirectory))
            {
                LogWarning("No AuditTrails directory found.");
                return;
            }

            var logFiles = Directory.GetFiles(_auditTrailDirectory, "*.json");
            if (!logFiles.Any())
            {
                LogWarning("No log files found in AuditTrails directory.");
                return;
            }

            LogInfo($"Found {logFiles.Length} log files.");

            foreach (var file in logFiles)
            {
                await DisplayLogFileAsync(file, "Audit Trail");
            }
        }

        private async Task SearchAuditLogsByChassisAsync()
        {
            Console.Clear();
            LogStep("Search Audit Trail Logs by Chassis Number");

            if (!Directory.Exists(_auditTrailDirectory))
            {
                LogWarning("No AuditTrails directory found.");
                return;
            }

            Console.Write("Enter Chassis Number to search: ");
            var chassisNumber = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(chassisNumber))
            {
                LogError("Invalid chassis number entered.");
                return;
            }

            var logFiles = Directory.GetFiles(_auditTrailDirectory, $"{chassisNumber}_*.json");

            if (!logFiles.Any())
            {
                LogWarning($"No logs found for Chassis: {chassisNumber}");

                var allFiles = Directory.GetFiles(_auditTrailDirectory, "*.json");
                if (allFiles.Any())
                {
                    LogInfo("Available chassis numbers in logs:");
                    var chassisNumbers = allFiles
                        .Select(f => Path.GetFileNameWithoutExtension(f).Split('_')[0])
                        .Distinct()
                        .OrderBy(v => v)
                        .Take(20);

                    foreach (var v in chassisNumbers)
                    {
                        LogInfo($"  - {v}");
                    }
                    if (allFiles.Length > 20)
                    {
                        LogInfo($"  ... and {allFiles.Length - 20} more");
                    }
                }
                return;
            }

            LogSuccess($"Found {logFiles.Length} log file(s) for Chassis: {chassisNumber}");

            foreach (var file in logFiles)
            {
                await DisplayLogFileAsync(file, "Audit Trail");
            }
        }

        private async Task ViewAllFailedRequestsAsync()
        {
            Console.Clear();
            LogStep("Failed Requests (Dead Letters)");

            if (!Directory.Exists(_deadLetterDirectory))
            {
                LogWarning("No FailedRequests directory found.");
                return;
            }

            var deadLetterFiles = Directory.GetFiles(_deadLetterDirectory, "*.json");
            if (!deadLetterFiles.Any())
            {
                LogWarning("No failed request files found.");
                return;
            }

            LogInfo($"Found {deadLetterFiles.Length} failed request file(s).");

            foreach (var file in deadLetterFiles)
            {
                await DisplayLogFileAsync(file, "Failed Request");
            }
        }

        private async Task SearchFailedRequestsByChassisAsync()
        {
            Console.Clear();
            LogStep("Search Failed Requests by Chassis Number");

            if (!Directory.Exists(_deadLetterDirectory))
            {
                LogWarning("No FailedRequests directory found.");
                return;
            }

            Console.Write("Enter Chassis Number to search: ");
            var chassisNumber = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(chassisNumber))
            {
                LogError("Invalid chassis number entered.");
                return;
            }

            var deadLetterFiles = Directory.GetFiles(_deadLetterDirectory, "*.json");
            var foundEntries = new List<(string File, object Entry)>();

            foreach (var file in deadLetterFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    var jsonDoc = JsonDocument.Parse(content);

                    if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var element in jsonDoc.RootElement.EnumerateArray())
                        {
                            if (ContainsChassis(element, chassisNumber))
                            {
                                foundEntries.Add((file, element));
                            }
                        }
                    }
                    else
                    {
                        if (ContainsChassis(jsonDoc.RootElement, chassisNumber))
                        {
                            foundEntries.Add((file, jsonDoc.RootElement));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error reading file {file}: {ex.Message}");
                }
            }

            if (!foundEntries.Any())
            {
                LogWarning($"No failed request entries found for Chassis: {chassisNumber}");
                return;
            }

            LogSuccess($"Found {foundEntries.Count} failed request entry(ies) for Chassis: {chassisNumber}");

            foreach (var (file, entry) in foundEntries)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"File: {Path.GetFileName(file)}");
                Console.WriteLine(new string('=', 80));
                Console.ResetColor();

                var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
        }

        private async Task ViewStatisticsAsync()
        {
            Console.Clear();
            LogStep("Summary Statistics - Loan Management System");

            if (Directory.Exists(_auditTrailDirectory))
            {
                var logFiles = Directory.GetFiles(_auditTrailDirectory, "*.json");
                var totalApplicants = logFiles
                    .Select(f => Path.GetFileNameWithoutExtension(f).Split('_')[0])
                    .Distinct()
                    .Count();

                LogInfo($"Audit Trail Logs:");
                LogInfo($"  - Total log files: {logFiles.Length}");
                LogInfo($"  - Unique Applicants: {totalApplicants}");

                var successCount = 0;
                var failureCount = 0;
                var attemptCounts = new Dictionary<int, int>();

                foreach (var file in logFiles)
                {
                    try
                    {
                        var content = await File.ReadAllTextAsync(file);
                        var entries = ParseLogEntries(content);

                        foreach (var entry in entries)
                        {
                            if (entry.TryGetProperty("IsSuccess", out var successProp))
                            {
                                if (successProp.GetBoolean())
                                    successCount++;
                                else
                                    failureCount++;
                            }

                            if (entry.TryGetProperty("Attempt", out var attemptProp))
                            {
                                var attempt = attemptProp.GetInt32();
                                if (!attemptCounts.ContainsKey(attempt))
                                    attemptCounts[attempt] = 0;
                                attemptCounts[attempt]++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error reading file {file}: {ex.Message}");
                    }
                }

                LogInfo($"  - Successful requests: {successCount}");
                LogInfo($"  - Failed requests: {failureCount}");
                LogInfo($"  - Success Rate: {(successCount + failureCount > 0 ? (successCount / (double)(successCount + failureCount) * 100).ToString("F2") : "N/A")}%");
                LogInfo($"  - Attempt distribution:");
                foreach (var kvp in attemptCounts.OrderBy(k => k.Key))
                {
                    LogInfo($"      Attempt {kvp.Key}: {kvp.Value} requests");
                }
            }
            else
            {
                LogWarning("No AuditTrails directory found.");
            }

            Console.WriteLine();

            if (Directory.Exists(_deadLetterDirectory))
            {
                var deadLetterFiles = Directory.GetFiles(_deadLetterDirectory, "*.json");
                var totalFailed = 0;

                foreach (var file in deadLetterFiles)
                {
                    try
                    {
                        var content = await File.ReadAllTextAsync(file);
                        var jsonDoc = JsonDocument.Parse(content);

                        if (jsonDoc.RootElement.TryGetProperty("TotalFailed", out var totalProp))
                        {
                            totalFailed += totalProp.GetInt32();
                        }
                        else if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            totalFailed += jsonDoc.RootElement.GetArrayLength();
                        }
                        else if (jsonDoc.RootElement.TryGetProperty("FailedItems", out var itemsProp))
                        {
                            totalFailed += itemsProp.GetArrayLength();
                        }
                        else
                        {
                            totalFailed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error reading file {file}: {ex.Message}");
                    }
                }

                LogInfo($"Failed Requests:");
                LogInfo($"  - Total files: {deadLetterFiles.Length}");
                LogInfo($"  - Total failed items: {totalFailed}");
            }
            else
            {
                LogWarning("No FailedRequests directory found.");
            }

            // Show processed count
            var processedFile = Path.Combine(AppContext.BaseDirectory, "ProcessedApplicants.txt");
            if (File.Exists(processedFile))
            {
                var lines = await File.ReadAllLinesAsync(processedFile);
                var validLines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
                LogInfo($"");
                LogInfo($"Processed Applicants:");
                LogInfo($"  - Total processed: {validLines.Count}");
            }
        }

        private async Task DisplayLogFileAsync(string filePath, string logType)
        {
            try
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"File: {Path.GetFileName(filePath)}");
                Console.WriteLine($"Type: {logType}");
                Console.WriteLine(new string('=', 80));
                Console.ResetColor();

                var content = await File.ReadAllTextAsync(filePath);

                if (content.TrimStart().StartsWith("{"))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(content);
                        var formattedJson = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                        Console.WriteLine(formattedJson);
                    }
                    catch (JsonException)
                    {
                        var lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                try
                                {
                                    var jsonDoc = JsonDocument.Parse(line);
                                    var formattedJson = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                                    Console.WriteLine(formattedJson);
                                    Console.WriteLine(new string('-', 80));
                                }
                                catch (JsonException)
                                {
                                    Console.WriteLine(line);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                var jsonDoc = JsonDocument.Parse(line);
                                var formattedJson = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                                Console.WriteLine(formattedJson);
                                Console.WriteLine(new string('-', 80));
                            }
                            catch (JsonException)
                            {
                                Console.WriteLine(line);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error displaying file {filePath}: {ex.Message}");
            }
        }

        private List<JsonElement> ParseLogEntries(string content)
        {
            var entries = new List<JsonElement>();

            try
            {
                if (content.TrimStart().StartsWith("{"))
                {
                    var jsonDoc = JsonDocument.Parse(content);
                    entries.Add(jsonDoc.RootElement);
                }
                else
                {
                    var lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                var jsonDoc = JsonDocument.Parse(line);
                                entries.Add(jsonDoc.RootElement);
                            }
                            catch (JsonException) { }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var element in jsonDoc.RootElement.EnumerateArray())
                        {
                            entries.Add(element);
                        }
                    }
                }
                catch { }
            }

            return entries;
        }

        private bool ContainsChassis(JsonElement element, string chassisNumber)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                if (element.TryGetProperty("ChassisNumber", out var chassisProp) &&
                    chassisProp.ValueKind == JsonValueKind.String)
                {
                    return chassisProp.GetString()?.Equals(chassisNumber, StringComparison.OrdinalIgnoreCase) == true;
                }

                if (element.TryGetProperty("Chassis", out var chassisProp2) &&
                    chassisProp2.ValueKind == JsonValueKind.String)
                {
                    return chassisProp2.GetString()?.Equals(chassisNumber, StringComparison.OrdinalIgnoreCase) == true;
                }

                if (element.TryGetProperty("FailedItems", out var itemsProp) &&
                    itemsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in itemsProp.EnumerateArray())
                    {
                        if (ContainsChassis(item, chassisNumber))
                            return true;
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (ContainsChassis(item, chassisNumber))
                        return true;
                }
            }

            return false;
        }

        private void LogInfo(string message) => ConsoleLogger.LogInfo(message);
        private void LogSuccess(string message) => ConsoleLogger.LogSuccess(message);
        private void LogWarning(string message) => ConsoleLogger.LogWarning(message);
        private void LogError(string message) => ConsoleLogger.LogError(message);
        private void LogDebug(string message) => ConsoleLogger.LogDebug(message);
        private void LogStep(string message) => ConsoleLogger.LogStep(message);
        private void LogProgress(string message) => ConsoleLogger.LogProgress(message);
    }
}
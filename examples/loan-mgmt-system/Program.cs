using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using LoanManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem;

class Program
{
    private static IServiceProvider _serviceProvider;

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    ########## Welcome ##########           ║");
        Console.WriteLine("║              Loan Management Processing System             ║");
        Console.WriteLine("║                      Auto Loan Division                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        SetupHost();

        using var scope = _serviceProvider.CreateScope();
        var processingEngine = new LoanProcessingEngine();
        var logViewer = new LogViewerService();

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                    MAIN MENU                           │");
            Console.WriteLine("├─────────────────────────────────────────────────────────┤");
            Console.WriteLine("│  1. Process Loan Approval Requests  (Full Run)         │");
            Console.WriteLine("│  2. Process Loan Cancellation Req.  (Full Run)         │");
            Console.WriteLine("│  3. Test First Approval Request    (Test Mode)         │");
            Console.WriteLine("│  4. Test First Cancellation Req.   (Test Mode)         │");
            Console.WriteLine("│  5. Audit Log Viewer                                  │");
            Console.WriteLine("│  6. Exit                                              │");
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
                        await processingEngine.ProcessLoanApprovalRequestsAsync();
                        break;
                    case 2:
                        await processingEngine.ProcessLoanCancellationRequestsAsync();
                        break;
                    case 3:
                        var approvalTestResult = await processingEngine.TestFirstApprovalRequestAsync();
                        if (approvalTestResult)
                        {
                            LogSuccess("Loan approval test completed successfully!");
                        }
                        else
                        {
                            LogError("Loan approval test failed!");
                        }
                        break;
                    case 4:
                        var cancellationTestResult = await processingEngine.TestFirstCancellationRequestAsync();
                        if (cancellationTestResult)
                        {
                            LogSuccess("Loan cancellation test completed successfully!");
                        }
                        else
                        {
                            LogError("Loan cancellation test failed!");
                        }
                        break;
                    case 5:
                        await logViewer.ViewAllLogsAsync();
                        break;
                    case 6:
                        LogSuccess("Exiting application... Goodbye!");
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

    private static void SetupHost()
    {
        try
        {
            LogInfo("Initializing application...");

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<LoanManagementDbContext>((provider, options) =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                        options.UseSqlServer(connectionString, sqlOptions =>
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null
                            )
                        );
                    });
                })
                .Build();

            _serviceProvider = host.Services;
            LogSuccess("Application initialized successfully!");
        }
        catch (Exception ex)
        {
            LogError($"Failed to initialize application: {ex.Message}");
            throw;
        }
    }

    private static void LogInfo(string message) => ConsoleLogger.LogInfo(message);
    private static void LogSuccess(string message) => ConsoleLogger.LogSuccess(message);
    private static void LogWarning(string message) => ConsoleLogger.LogWarning(message);
    private static void LogError(string message) => ConsoleLogger.LogError(message);
    private static void LogDebug(string message) => ConsoleLogger.LogDebug(message);
    private static void LogStep(string message) => ConsoleLogger.LogStep(message);
    private static void LogProgress(string message) => ConsoleLogger.LogProgress(message);
}
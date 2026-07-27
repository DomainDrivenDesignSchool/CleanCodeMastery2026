namespace LoanManagementSystem
{
    public static class ConsoleLogger
    {
        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[INFO] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SUCCESS] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[WARNING] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[DEBUG] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogStep(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("[STEP] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogProgress(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[PROGRESS] ");
            Console.ResetColor();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}
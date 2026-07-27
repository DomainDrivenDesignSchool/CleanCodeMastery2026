using ClosedXML.Excel;

namespace LoanManagementSystem
{
    public static class ExcelCellHelper
    {
        public static int GetIntSafe(this IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return 0;

            try
            {
                if (cell.TryGetValue(out int intValue))
                    return intValue;

                if (cell.TryGetValue(out double doubleValue))
                    return (int)doubleValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return 0;

                if (int.TryParse(stringValue, out int parsedInt))
                    return parsedInt;

                if (double.TryParse(stringValue, out double parsedDouble))
                    return (int)parsedDouble;

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetIntOrDefault(this IXLCell cell, int defaultValue = 0)
        {
            if (cell == null || cell.IsEmpty())
                return defaultValue;

            try
            {
                if (cell.TryGetValue(out int intValue))
                    return intValue;

                if (cell.TryGetValue(out double doubleValue))
                    return (int)doubleValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return defaultValue;

                if (int.TryParse(stringValue, out int parsedInt))
                    return parsedInt;

                if (double.TryParse(stringValue, out double parsedDouble))
                    return (int)parsedDouble;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int? GetIntNullable(this IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return null;

            try
            {
                if (cell.TryGetValue(out int intValue))
                    return intValue;

                if (cell.TryGetValue(out double doubleValue))
                    return (int)doubleValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;

                if (int.TryParse(stringValue, out int parsedInt))
                    return parsedInt;

                if (double.TryParse(stringValue, out double parsedDouble))
                    return (int)parsedDouble;

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string GetStringSafe(this IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return string.Empty;

            try
            {
                return cell.GetString()?.Trim() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetStringOrNull(this IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return null;

            try
            {
                var value = cell.GetString()?.Trim();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetDateTimeSafe(this IXLCell cell, DateTime defaultValue = default)
        {
            if (cell == null || cell.IsEmpty())
                return defaultValue;

            try
            {
                if (cell.TryGetValue(out DateTime dateTimeValue))
                    return dateTimeValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return defaultValue;

                if (DateTime.TryParse(stringValue, out DateTime parsedDateTime))
                    return parsedDateTime;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime? GetDateTimeNullable(this IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return null;

            try
            {
                if (cell.TryGetValue(out DateTime dateTimeValue))
                    return dateTimeValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;

                if (DateTime.TryParse(stringValue, out DateTime parsedDateTime))
                    return parsedDateTime;

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool GetBoolSafe(this IXLCell cell, bool defaultValue = false)
        {
            if (cell == null || cell.IsEmpty())
                return defaultValue;

            try
            {
                if (cell.TryGetValue(out bool boolValue))
                    return boolValue;

                var stringValue = cell.GetString().Trim().ToLower();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return defaultValue;

                if (stringValue == "true" || stringValue == "1" || stringValue == "yes")
                    return true;

                if (stringValue == "false" || stringValue == "0" || stringValue == "no")
                    return false;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static double GetDoubleSafe(this IXLCell cell, double defaultValue = 0)
        {
            if (cell == null || cell.IsEmpty())
                return defaultValue;

            try
            {
                if (cell.TryGetValue(out double doubleValue))
                    return doubleValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return defaultValue;

                if (double.TryParse(stringValue, out double parsedDouble))
                    return parsedDouble;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal GetDecimalSafe(this IXLCell cell, decimal defaultValue = 0)
        {
            if (cell == null || cell.IsEmpty())
                return defaultValue;

            try
            {
                if (cell.TryGetValue(out decimal decimalValue))
                    return decimalValue;

                if (cell.TryGetValue(out double doubleValue))
                    return (decimal)doubleValue;

                var stringValue = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return defaultValue;

                if (decimal.TryParse(stringValue, out decimal parsedDecimal))
                    return parsedDecimal;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
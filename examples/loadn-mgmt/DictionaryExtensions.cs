using ClosedXML.Excel;
using System;

namespace LoanManagementSystem
{
    public static class ColumnMapExtensions
    {
        public static int GetColumnIndex(this Dictionary<string, int> columnMap, string key, int defaultValue = -1)
        {
            return columnMap.TryGetValue(key, out int value) ? value : defaultValue;
        }

        public static bool HasColumn(this Dictionary<string, int> columnMap, string key)
        {
            return columnMap.ContainsKey(key);
        }
    }

    public static class ExcelRowHelper
    {
        public static int GetInt(this IXLRow row, Dictionary<string, int> columnMap, string columnName, int defaultValue = 0)
        {
            if (!columnMap.TryGetValue(columnName, out int colIndex))
                return defaultValue;

            var cell = row.Cell(colIndex);
            return cell.GetIntSafe();
        }

        public static string GetString(this IXLRow row, Dictionary<string, int> columnMap, string columnName, string defaultValue = "")
        {
            if (!columnMap.TryGetValue(columnName, out int colIndex))
                return defaultValue;

            var cell = row.Cell(colIndex);
            return cell.GetStringSafe();
        }

        public static DateTime GetDateTime(this IXLRow row, Dictionary<string, int> columnMap, string columnName, DateTime defaultValue = default)
        {
            if (!columnMap.TryGetValue(columnName, out int colIndex))
                return defaultValue;

            var cell = row.Cell(colIndex);
            return cell.GetDateTimeSafe(defaultValue);
        }

        public static double GetDouble(this IXLRow row, Dictionary<string, int> columnMap, string columnName, double defaultValue = 0)
        {
            if (!columnMap.TryGetValue(columnName, out int colIndex))
                return defaultValue;

            var cell = row.Cell(colIndex);
            return cell.GetDoubleSafe(defaultValue);
        }

        public static bool GetBool(this IXLRow row, Dictionary<string, int> columnMap, string columnName, bool defaultValue = false)
        {
            if (!columnMap.TryGetValue(columnName, out int colIndex))
                return defaultValue;

            var cell = row.Cell(colIndex);
            return cell.GetBoolSafe(defaultValue);
        }
    }
}
using ClosedXML.Excel;
using System.Globalization;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

/// <summary>
/// Shared helpers for Excel cell reading and value parsing.
/// Extend this class to add reusable logic across handlers.
/// </summary>
public abstract class ImportHandlerBase
{
    // --- Sheet discovery ---

    protected static IXLWorksheet FindSheet(IXLWorkbook workbook, string preferredName)
        => workbook.TryGetWorksheet(preferredName, out var sheet) ? sheet : workbook.Worksheets.First();

    // --- Header mapping ---

    /// <summary>
    /// Reads the first row and returns a map of header text → 1-based column number.
    /// Matching is case-insensitive for robustness when files are edited manually.
    /// </summary>
    protected static Dictionary<string, int> ReadHeaderMap(IXLWorksheet sheet)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in sheet.Row(1).CellsUsed())
        {
            var text = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(text) && !map.ContainsKey(text))
                map[text] = cell.Address.ColumnNumber;
        }
        return map;
    }

    // --- Cell readers: read typed values from a row using the header map ---

    protected static string? GetCellString(IXLRow row, Dictionary<string, int> map, string column)
    {
        if (!map.TryGetValue(column, out var col)) return null;
        var text = row.Cell(col).GetString()?.Trim();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    protected static decimal? GetCellDecimal(IXLRow row, Dictionary<string, int> map, string column)
    {
        if (!map.TryGetValue(column, out var col)) return null;
        var cell = row.Cell(col);
        if (cell.Value.IsNumber) return (decimal)cell.Value.GetNumber();
        var text = cell.GetString()?.Trim();
        if (string.IsNullOrWhiteSpace(text)) return null;
        // Accept both invariant (.) and current culture (,) decimal separators for manual edits
        if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
        if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;
        return null;
    }

    protected static int GetCellInt(IXLRow row, Dictionary<string, int> map, string column, int defaultValue = 0)
    {
        if (!map.TryGetValue(column, out var col)) return defaultValue;
        var cell = row.Cell(col);
        if (cell.Value.IsNumber) return (int)cell.Value.GetNumber();
        var text = cell.GetString()?.Trim();
        return int.TryParse(text, out var i) ? i : defaultValue;
    }

    protected static bool GetCellBool(IXLRow row, Dictionary<string, int> map, string column)
    {
        if (!map.TryGetValue(column, out var col)) return false;
        var cell = row.Cell(col);
        // ClosedXML uses IsText for everything in newer versions; parse the string representation robustly.
        var text = cell.GetString()?.Trim();
        return ParseBoolString(text);
    }

    // --- Value parsers ---

    protected static Guid? ParseGuid(string? value)
        => Guid.TryParse(value, out var g) ? g : null;

    protected static bool ParseBoolString(string? value)
        => value is not null
            && (value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value == "1"
                || value.Equals("ja", StringComparison.OrdinalIgnoreCase)
                || value.Equals("yes", StringComparison.OrdinalIgnoreCase));

    protected static DateOnly? ParseDateOnly(string? value)
        => DateOnly.TryParse(value, out var d) ? d : null;

    protected static TimeOnly? ParseTimeOnly(string? value)
        => TimeOnly.TryParse(value, out var t) ? t : null;

    protected static int[]? ParseIntArray(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        try
        {
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim()))
                .ToArray();
        }
        catch { return null; }
    }

    // --- Display helpers ---

    /// <summary>Shorthand for creating a display column entry.</summary>
    protected static KeyValuePair<string, string> D(string key, string? value)
        => new(key, value ?? string.Empty);
}

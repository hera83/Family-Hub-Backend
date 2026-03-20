using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Sync;

namespace FamilyHub.Adm.Services.ImportExport;

/// <summary>
/// Generates downloadable Excel import templates.
/// Color convention on the data sheet header row:
///   Yellow  (#FFD966) = påkrævet felt (required)
///   Blue    (#9DC3E6) = Id/FK-referencefelt (ID or foreign key)
///   Gray    (#EDEDED) = valgfrit felt (optional)
///   Dark gray (#BFBFBF) = ignoreret ved import (readonly/audit field)
/// </summary>
public sealed class ExcelTemplateService(ISyncApiClient syncApiClient) : IExcelTemplateService
{
    private readonly ISyncApiClient _syncApiClient = syncApiClient;

    // ── Column roles ────────────────────────────────────────────────────────────
    private enum Role { Required, FK, Optional, Ignored }

    private sealed record Col(
        string Name,
        Role Role,
        string Type,
        string Description,
        string Example = "");

    // ── Color constants ─────────────────────────────────────────────────────────
    private static readonly XLColor ColRequired = XLColor.FromHtml("#FFD966"); // yellow
    private static readonly XLColor ColFK       = XLColor.FromHtml("#9DC3E6"); // blue
    private static readonly XLColor ColOptional = XLColor.FromHtml("#EDEDED"); // light gray
    private static readonly XLColor ColIgnored  = XLColor.FromHtml("#BFBFBF"); // dark gray

    // ═══════════════════════════════════════════════════════════════════════════
    // Public interface
    // ═══════════════════════════════════════════════════════════════════════════

    public Task<ExcelExportFile> GetFamilyMembersTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",           Role.FK,       "GUID",   "Udelad for ny post; angiv eksisterende GUID for opdatering.",  ""),
            new("Name",         Role.Required, "tekst",  "Navn på familiemedlemmet.",                                    "Mia Andersen"),
            new("Color",        Role.Required, "hex-farve", "Farve i hex-format, fx #E91E63.",                           "#E91E63"),
            new("CreatedAtUtc", Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",                  ""),
            new("UpdatedAtUtc", Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",                  ""),
        ];

        string[] example = ["", "Mia Andersen", "#E91E63", "", ""];

        var bytes = BuildTemplate("FamilyMembers", "Familiemedlemmer", cols, [example],
            extraSheets: null);
        return Task.FromResult(CreateFile("family-members-template", bytes));
    }

    public async Task<ExcelExportFile> GetCalendarEventsTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",               Role.FK,       "GUID",     "Udelad for ny post; angiv GUID for opdatering.",         ""),
            new("Title",            Role.Required, "tekst",    "Titel på begivenheden.",                                 "Lægebesøg"),
            new("Description",      Role.Optional, "tekst",    "Valgfri beskrivelse.",                                   "Tjek hos lægen"),
            new("EventDate",        Role.Required, "dato",     "Dato i formatet yyyy-MM-dd.",                            "2026-04-15"),
            new("StartTime",        Role.Optional, "tid",      "Starttidspunkt i formatet HH:mm:ss.",                    "10:00:00"),
            new("EndTime",          Role.Optional, "tid",      "Sluttidspunkt i formatet HH:mm:ss.",                     "10:30:00"),
            new("FamilyMemberId",   Role.FK,       "GUID",     "FK til familiemedlem. Se 'Familiemedlemmer'-arket.",      ""),
            new("FamilyMemberName", Role.FK,       "tekst",    "Navn på familiemedlem som alternativ til FamilyMemberId.", "Mia Andersen"),
            new("RecurrenceType",   Role.Optional, "tekst",    "None | Daily | Weekly | Monthly",                       "None"),
            new("RecurrenceDays",   Role.Optional, "tekst",    "Kommaseparerede ugedagsnumre, fx '1,3,5' (man,ons,fre).", ""),
            new("CreatedAtUtc",     Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",              ""),
            new("UpdatedAtUtc",     Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",              ""),
        ];

        string[] example = ["", "Lægebesøg", "Tjek hos lægen", "2026-04-15", "10:00:00", "10:30:00", "", "Mia Andersen", "None", "", "", ""];

        var lookupSheet = await TryBuildFamilyMemberLookupAsync(ct);
        var bytes = BuildTemplate("CalendarEvents", "Kalenderbegivenheder", cols, [example],
            extraSheets: lookupSheet is not null ? [lookupSheet] : null);
        return CreateFile("calendar-events-template", bytes);
    }

    public Task<ExcelExportFile> GetItemCategoriesTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",           Role.FK,       "GUID",   "Udelad for ny post; angiv GUID for opdatering.",    ""),
            new("Name",         Role.Required, "tekst",  "Navn på madvarekategorien.",                        "Mejeriprodukter"),
            new("SortOrder",    Role.Optional, "heltal", "Sorteringsorden (laveste vises først). Standard 0.", "10"),
            new("CreatedAtUtc", Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",       ""),
            new("UpdatedAtUtc", Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",       ""),
        ];

        string[] example = ["", "Mejeriprodukter", "10", "", ""];

        var bytes = BuildTemplate("ItemCategories", "Madvarekategorier", cols, [example],
            extraSheets: null);
        return Task.FromResult(CreateFile("item-categories-template", bytes));
    }

    public async Task<ExcelExportFile> GetProductsTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",               Role.FK,       "GUID",    "Udelad for ny post; angiv GUID for opdatering.",              ""),
            new("Name",             Role.Required, "tekst",   "Navn på madvaren.",                                           "Sødmælk"),
            new("ItemCategoryId",   Role.FK,       "GUID",    "FK til madvarekategori. Se 'Madvarekategorier'-arket.",        ""),
            new("ItemCategoryName", Role.FK,       "tekst",   "Kategorinavn som alternativ til ItemCategoryId.",             "Mejeriprodukter"),
            new("Description",      Role.Optional, "tekst",   "Valgfri beskrivelse.",                                        ""),
            new("ImageUrl",         Role.Optional, "URL",     "URL til produktbillede.",                                     ""),
            new("Unit",             Role.Optional, "tekst",   "Enhed, fx 'stk', 'liter', 'kg'.",                            "liter"),
            new("SizeLabel",        Role.Optional, "tekst",   "Størrelsesangivelse, fx '1 liter', '500 g'.",                 "1 liter"),
            new("Price",            Role.Optional, "decimal", "Pris (kan efterlades tom).",                                  "12.95"),
            new("IsManual",         Role.Optional, "TRUE/FALSE", "TRUE = manuelt oprettet produkt.",                         "FALSE"),
            new("IsFavorite",       Role.Optional, "TRUE/FALSE", "TRUE = markeret som favorit.",                             "FALSE"),
            new("IsStaple",         Role.Optional, "TRUE/FALSE", "TRUE = basisvare.",                                        "FALSE"),
            new("CaloriesPer100g",  Role.Optional, "decimal", "Kalorier pr. 100 g.",                                        "61"),
            new("FatPer100g",       Role.Optional, "decimal", "Fedt pr. 100 g.",                                            "3.5"),
            new("CarbsPer100g",     Role.Optional, "decimal", "Kulhydrat pr. 100 g.",                                       "4.7"),
            new("ProteinPer100g",   Role.Optional, "decimal", "Protein pr. 100 g.",                                         "3.3"),
            new("FiberPer100g",     Role.Optional, "decimal", "Kostfibre pr. 100 g.",                                       "0"),
            new("CreatedAtUtc",     Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",                  ""),
            new("UpdatedAtUtc",     Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",                  ""),
        ];

        string[] example = ["", "Sødmælk", "", "Mejeriprodukter", "", "", "liter", "1 liter", "12.95", "FALSE", "FALSE", "FALSE", "61", "3.5", "4.7", "3.3", "0", "", ""];

        var lookupSheet = await TryBuildItemCategoryLookupAsync(ct);
        var bytes = BuildTemplate("Products", "Madvarer", cols, [example],
            extraSheets: lookupSheet is not null ? [lookupSheet] : null);
        return CreateFile("products-template", bytes);
    }

    public Task<ExcelExportFile> GetRecipeCategoriesTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",           Role.FK,       "GUID",    "Udelad for ny post; angiv GUID for opdatering.",    ""),
            new("Name",         Role.Required, "tekst",   "Navn på opskriftskategorien.",                      "Morgenmad"),
            new("SortOrder",    Role.Optional, "heltal",  "Sorteringsorden (laveste vises først). Standard 0.", "10"),
            new("CreatedAtUtc", Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",        ""),
            new("UpdatedAtUtc", Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",        ""),
        ];

        string[] example = ["", "Morgenmad", "10", "", ""];

        var bytes = BuildTemplate("RecipeCategories", "Opskriftskategorier", cols, [example],
            extraSheets: null);
        return Task.FromResult(CreateFile("recipe-categories-template", bytes));
    }

    public async Task<ExcelExportFile> GetRecipesTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",                 Role.FK,       "GUID",    "Udelad for ny post; angiv GUID for opdatering.",             ""),
            new("Title",              Role.Required, "tekst",   "Titel på opskriften.",                                       "Havregrød"),
            new("RecipeCategoryId",   Role.FK,       "GUID",    "FK til opskriftskategori. Se 'Opskriftskategorier'-arket.",   ""),
            new("RecipeCategoryName", Role.FK,       "tekst",   "Kategorinavn som alternativ til RecipeCategoryId.",          "Morgenmad"),
            new("ImageUrl",           Role.Optional, "URL",     "URL til opskriftsbillede.",                                  ""),
            new("Description",        Role.Optional, "tekst",   "Kort beskrivelse af opskriften.",                            "Varm og mættende grød"),
            new("PrepTimeMinutes",    Role.Optional, "heltal",  "Forberedelsestid i minutter.",                               "5"),
            new("WaitTimeMinutes",    Role.Optional, "heltal",  "Ventetid (bagetid, osv.) i minutter.",                       "10"),
            new("Instructions",       Role.Optional, "tekst",   "Fremgangsmåde. Kan indeholde linjeskift.",                   "Kog vand og tilsæt havre."),
            new("IsManual",           Role.Optional, "TRUE/FALSE", "TRUE = manuelt oprettet opskrift.",                       "FALSE"),
            new("IsFavorite",         Role.Optional, "TRUE/FALSE", "TRUE = markeret som favorit.",                            "FALSE"),
            new("CreatedAtUtc",       Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",                 ""),
            new("UpdatedAtUtc",       Role.Ignored,  "dato/tid", "Sættes automatisk — ignoreres ved import.",                 ""),
        ];

        string[] example = ["", "Havregrød", "", "Morgenmad", "", "Varm og mættende grød", "5", "10", "Kog vand og tilsæt havre.", "FALSE", "FALSE", "", ""];

        var lookupSheet = await TryBuildRecipeCategoryLookupAsync(ct);
        var bytes = BuildTemplate("Recipes", "Opskrifter", cols, [example],
            extraSheets: lookupSheet is not null ? [lookupSheet] : null);
        return CreateFile("recipes-template", bytes);
    }

    public async Task<ExcelExportFile> GetRecipeIngredientsTemplateAsync(CancellationToken ct = default)
    {
        Col[] cols =
        [
            new("Id",          Role.FK,       "GUID",    "Udelad for ny post; angiv GUID for opdatering.",                ""),
            new("RecipeId",    Role.FK,       "GUID",    "FK til opskrift. Se 'Opskrifter'-arket. Påkrævet hvis RecipeTitle er tom.", ""),
            new("RecipeTitle", Role.FK,       "tekst",   "Opskriftens titel som alternativ til RecipeId.",                "Havregrød"),
            new("ProductId",   Role.FK,       "GUID",    "FK til madvarelookup. Se 'Madvarer'-arket. Valgfrit.",          ""),
            new("ProductName", Role.FK,       "tekst",   "Madvare-navn som alternativ til ProductId. Valgfrit.",          "Havregryn"),
            new("Name",        Role.Optional, "tekst",   "Fritekst-navn på ingrediensen (bruges hvis ProductId og ProductName er tomme).", "Havregryn"),
            new("Quantity",    Role.Optional, "decimal", "Mængde.",                                                       "100"),
            new("Unit",        Role.Optional, "tekst",   "Enhed, fx 'g', 'dl', 'stk'.",                                  "g"),
            new("IsStaple",    Role.Optional, "TRUE/FALSE", "TRUE = basisingrediens.",                                    "FALSE"),
            new("SortOrder",   Role.Optional, "heltal",  "Sorteringsorden inden for opskriften.",                         "1"),
            new("CreatedAtUtc", Role.Ignored, "dato/tid", "Sættes automatisk — ignoreres ved import.",                   ""),
            new("UpdatedAtUtc", Role.Ignored, "dato/tid", "Sættes automatisk — ignoreres ved import.",                   ""),
        ];

        // Rule: RecipeId OR RecipeTitle required; ProductId/ProductName/Name at least one.
        string[] example = ["", "", "Havregrød", "", "Havregryn", "", "100", "g", "FALSE", "1", "", ""];

        var (recipesSheet, productsSheet) = await TryBuildIngredientLookupsAsync(ct);
        var extra = new List<Action<XLWorkbook>>();
        if (recipesSheet is not null) extra.Add(recipesSheet);
        if (productsSheet is not null) extra.Add(productsSheet);

        var bytes = BuildTemplate("RecipeIngredients", "Opskriftsingredienser", cols, [example],
            extraSheets: extra.Count > 0 ? [.. extra] : null);
        return CreateFile("recipe-ingredients-template", bytes);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Core builder
    // ═══════════════════════════════════════════════════════════════════════════

    private static byte[] BuildTemplate(
        string sheetName,
        string displayName,
        Col[] cols,
        string[][] exampleRows,
        Action<XLWorkbook>[]? extraSheets)
    {
        using var wb = new XLWorkbook();

        AddDataSheet(wb, sheetName, cols, exampleRows);
        AddHelpSheet(wb, displayName, cols);

        if (extraSheets is not null)
            foreach (var addSheet in extraSheets)
                addSheet(wb);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ── Data sheet ──────────────────────────────────────────────────────────────

    private static void AddDataSheet(XLWorkbook wb, string sheetName, Col[] cols, string[][] exampleRows)
    {
        var sheet = wb.Worksheets.Add(sheetName);

        // Header row
        for (var i = 0; i < cols.Length; i++)
        {
            var col = cols[i];
            var cell = sheet.Cell(1, i + 1);
            cell.SetValue(col.Name);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = RoleColor(col.Role);
            if (col.Role == Role.Ignored)
                cell.Style.Font.Italic = true;
        }

        // Example rows
        for (var r = 0; r < exampleRows.Length; r++)
        {
            var row = exampleRows[r];
            for (var c = 0; c < row.Length && c < cols.Length; c++)
            {
                if (!string.IsNullOrEmpty(row[c]))
                    sheet.Cell(r + 2, c + 1).SetValue(row[c]);
            }
        }

        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();

        // Ensure minimum width for readability
        foreach (var xlCol in sheet.ColumnsUsed())
        {
            if (xlCol.Width < 14) xlCol.Width = 14;
        }
    }

    // ── Help sheet ──────────────────────────────────────────────────────────────

    private static void AddHelpSheet(XLWorkbook wb, string displayName, Col[] cols)
    {
        var sheet = wb.Worksheets.Add("Hjælp");
        var row = 1;

        // ── Title ──
        WriteTitle(sheet, row++, $"Import-hjælp – {displayName}");
        row++; // blank

        // ── Color legend ──
        WriteSectionHeader(sheet, row++, "Farveforklaring (kolonneoverskrifter)");

        WriteLegendRow(sheet, row++, ColRequired, "Påkrævet felt – skal udfyldes for at rækken er gyldig.");
        WriteLegendRow(sheet, row++, ColFK,       "Id / FK-referencefelt – bruges til opdatering eller FK-opslag.");
        WriteLegendRow(sheet, row++, ColOptional, "Valgfrit felt – kan udelades.");
        WriteLegendRow(sheet, row++, ColIgnored,  "Ignoreret ved import – sættes automatisk af systemet.");
        row++; // blank

        // ── Import rules ──
        WriteSectionHeader(sheet, row++, "Import-regler");

        WriteRuleRow(sheet, row++, "Ny post",
            "Lad Id-feltet være tomt. Systemet opretter en ny post og tildeler automatisk et Id.");
        WriteRuleRow(sheet, row++, "Opdater post",
            "Angiv et gyldigt GUID i Id-feltet. Systemet matcher på Id og opdaterer den eksisterende post.");
        WriteRuleRow(sheet, row++, "FK-felter",
            "For felter som fx ItemCategoryId / ItemCategoryName kan du enten angive GUID'et eller tekst-navnet. " +
            "Systemet bruger Id, hvis det er givet — ellers slår det tekst-navnet op. Se de yderligere Lookup-ark.");
        WriteRuleRow(sheet, row++, "Preview",
            "Ved upload vises et preview med validering. Rækker med fejl markeres men stopper ikke import af de øvrige rækker.");
        WriteRuleRow(sheet, row++, "Fejl",
            "Rækker der fejler validering (påkrævet felt mangler, ugyldig dato osv.) springes over. " +
            "Rækker der fejler under API-kaldet (fx duplikat) registreres som fejl i resultatsiden.");
        WriteRuleRow(sheet, row++, "Kolonnerækkefølge",
            "Kolonnerækkefølgen er ligeglad. Systemet finder kolonnerne ud fra navn i første række.");
        row++; // blank

        // ── Field reference ──
        WriteSectionHeader(sheet, row++, "Feltforklaring");

        // Table header
        var headerRow = row++;
        sheet.Cell(headerRow, 1).SetValue("Kolonne");
        sheet.Cell(headerRow, 2).SetValue("Rolle");
        sheet.Cell(headerRow, 3).SetValue("Type");
        sheet.Cell(headerRow, 4).SetValue("Forklaring");
        sheet.Cell(headerRow, 5).SetValue("Eksempel");
        var headerRange = sheet.Range(headerRow, 1, headerRow, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        foreach (var col in cols)
        {
            sheet.Cell(row, 1).SetValue(col.Name);
            sheet.Cell(row, 2).SetValue(RoleLabel(col.Role));
            sheet.Cell(row, 2).Style.Fill.BackgroundColor = RoleColor(col.Role);
            sheet.Cell(row, 3).SetValue(col.Type);
            sheet.Cell(row, 4).SetValue(col.Description);
            sheet.Cell(row, 5).SetValue(col.Example);
            row++;
        }

        sheet.Columns().AdjustToContents();
        // Ensure readable widths
        if (sheet.Column(4).Width < 50) sheet.Column(4).Width = 50;
        if (sheet.Column(1).Width < 22) sheet.Column(1).Width = 22;
        if (sheet.Column(5).Width < 20) sheet.Column(5).Width = 20;
    }

    // ── Lookup sheet builders ────────────────────────────────────────────────────

    private async Task<Action<XLWorkbook>?> TryBuildFamilyMemberLookupAsync(CancellationToken ct)
    {
        try
        {
            var members = await _syncApiClient.GetFullSyncAsync(ct);
            if (members.FamilyMembers.Count == 0) return null;
            return wb =>
            {
                var sheet = wb.Worksheets.Add("Familiemedlemmer");
                AddLookupHeader(sheet, "Id", "Name", "Color");
                var r = 2;
                foreach (var m in members.FamilyMembers.OrderBy(x => x.Name))
                {
                    sheet.Cell(r, 1).SetValue(m.Id.ToString());
                    sheet.Cell(r, 2).SetValue(m.Name);
                    sheet.Cell(r, 3).SetValue(m.Color);
                    r++;
                }
                FinalizeLookupSheet(sheet);
            };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { return null; }
    }

    private async Task<Action<XLWorkbook>?> TryBuildItemCategoryLookupAsync(CancellationToken ct)
    {
        try
        {
            var sync = await _syncApiClient.GetFullSyncAsync(ct);
            if (sync.ItemCategories.Count == 0) return null;
            return wb =>
            {
                var sheet = wb.Worksheets.Add("Madvarekategorier");
                AddLookupHeader(sheet, "Id", "Name", "SortOrder");
                var r = 2;
                foreach (var c in sync.ItemCategories.OrderBy(x => x.SortOrder).ThenBy(x => x.Name))
                {
                    sheet.Cell(r, 1).SetValue(c.Id.ToString());
                    sheet.Cell(r, 2).SetValue(c.Name);
                    sheet.Cell(r, 3).SetValue(c.SortOrder);
                    r++;
                }
                FinalizeLookupSheet(sheet);
            };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { return null; }
    }

    private async Task<Action<XLWorkbook>?> TryBuildRecipeCategoryLookupAsync(CancellationToken ct)
    {
        try
        {
            var sync = await _syncApiClient.GetFullSyncAsync(ct);
            if (sync.RecipeCategories.Count == 0) return null;
            return wb =>
            {
                var sheet = wb.Worksheets.Add("Opskriftskategorier");
                AddLookupHeader(sheet, "Id", "Name", "SortOrder");
                var r = 2;
                foreach (var c in sync.RecipeCategories.OrderBy(x => x.SortOrder).ThenBy(x => x.Name))
                {
                    sheet.Cell(r, 1).SetValue(c.Id.ToString());
                    sheet.Cell(r, 2).SetValue(c.Name);
                    sheet.Cell(r, 3).SetValue(c.SortOrder);
                    r++;
                }
                FinalizeLookupSheet(sheet);
            };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { return null; }
    }

    private async Task<(Action<XLWorkbook>? Recipes, Action<XLWorkbook>? Products)> TryBuildIngredientLookupsAsync(CancellationToken ct)
    {
        try
        {
            var sync = await _syncApiClient.GetFullSyncAsync(ct);

            Action<XLWorkbook>? recipesSheet = null;
            if (sync.Recipes.Count > 0)
            {
                recipesSheet = wb =>
                {
                    var sheet = wb.Worksheets.Add("Opskrifter");
                    AddLookupHeader(sheet, "Id", "Title");
                    var r = 2;
                    foreach (var rec in sync.Recipes.OrderBy(x => x.Title))
                    {
                        sheet.Cell(r, 1).SetValue(rec.Id.ToString());
                        sheet.Cell(r, 2).SetValue(rec.Title);
                        r++;
                    }
                    FinalizeLookupSheet(sheet);
                };
            }

            Action<XLWorkbook>? productsSheet = null;
            if (sync.Products.Count > 0)
            {
                productsSheet = wb =>
                {
                    var sheet = wb.Worksheets.Add("Madvarer");
                    AddLookupHeader(sheet, "Id", "Name", "Unit");
                    var r = 2;
                    foreach (var p in sync.Products.OrderBy(x => x.Name))
                    {
                        sheet.Cell(r, 1).SetValue(p.Id.ToString());
                        sheet.Cell(r, 2).SetValue(p.Name);
                        sheet.Cell(r, 3).SetValue(p.Unit ?? "");
                        r++;
                    }
                    FinalizeLookupSheet(sheet);
                };
            }

            return (recipesSheet, productsSheet);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { return (null, null); }
    }

    // ── Sheet helpers ────────────────────────────────────────────────────────────

    private static void WriteTitle(IXLWorksheet sheet, int row, string text)
    {
        var cell = sheet.Cell(row, 1);
        cell.SetValue(text);
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 14;
    }

    private static void WriteSectionHeader(IXLWorksheet sheet, int row, string text)
    {
        var cell = sheet.Cell(row, 1);
        cell.SetValue(text);
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 11;
        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        sheet.Range(row, 1, row, 5).Merge();
    }

    private static void WriteLegendRow(IXLWorksheet sheet, int row, XLColor color, string description)
    {
        var swatch = sheet.Cell(row, 1);
        swatch.SetValue("       ");
        swatch.Style.Fill.BackgroundColor = color;
        swatch.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        sheet.Cell(row, 2).SetValue(description);
        sheet.Range(row, 2, row, 5).Merge();
    }

    private static void WriteRuleRow(IXLWorksheet sheet, int row, string label, string text)
    {
        var labelCell = sheet.Cell(row, 1);
        labelCell.SetValue(label);
        labelCell.Style.Font.Bold = true;

        var textCell = sheet.Cell(row, 2);
        textCell.SetValue(text);
        textCell.Style.Alignment.WrapText = true;
        sheet.Range(row, 2, row, 5).Merge();
    }

    private static void AddLookupHeader(IXLWorksheet sheet, params string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var cell = sheet.Cell(1, i + 1);
            cell.SetValue(headers[i]);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        sheet.Cell(1, headers.Length + 2).SetValue("← Brug Id-kolonnen som reference i import-arket");
        sheet.Cell(1, headers.Length + 2).Style.Font.Italic = true;
        sheet.Cell(1, headers.Length + 2).Style.Font.FontColor = XLColor.Gray;
    }

    private static void FinalizeLookupSheet(IXLWorksheet sheet)
    {
        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();
        foreach (var col in sheet.ColumnsUsed())
            if (col.Width < 14) col.Width = 14;
    }

    // ── Utilities ────────────────────────────────────────────────────────────────

    private static XLColor RoleColor(Role role) => role switch
    {
        Role.Required => ColRequired,
        Role.FK       => ColFK,
        Role.Optional => ColOptional,
        Role.Ignored  => ColIgnored,
        _             => ColOptional,
    };

    private static string RoleLabel(Role role) => role switch
    {
        Role.Required => "Påkrævet",
        Role.FK       => "Id / FK-ref.",
        Role.Optional => "Valgfrit",
        Role.Ignored  => "Ignoreret",
        _             => "",
    };

    private static ExcelExportFile CreateFile(string name, byte[] content)
    {
        var fileName = $"familyhub-{name}.xlsx";
        return new ExcelExportFile(fileName, content);
    }
}

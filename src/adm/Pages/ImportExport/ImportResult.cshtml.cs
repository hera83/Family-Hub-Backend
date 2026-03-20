using FamilyHub.Adm.Services.ImportExport;
using FamilyHub.Adm.Services.ImportExport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.ImportExport;

public class ImportResultModel(IImportOrchestrator orchestrator) : PageModel
{
    private readonly IImportOrchestrator _orchestrator = orchestrator;

    public ImportResult? Result { get; private set; }
    public string? SessionExpiredMessage { get; private set; }

    public IActionResult OnGet()
    {
        var resultKey = TempData["ImportResultKey"] as string;
        if (string.IsNullOrWhiteSpace(resultKey))
        {
            TempData["ErrorMessage"] = "Ingen importresultat fundet.";
            return RedirectToPage("/ImportExport/Import");
        }

        Result = _orchestrator.GetCachedResult(resultKey);
        if (Result is null)
        {
            SessionExpiredMessage = "Resultat-sessionen er udløbet. Gå tilbage og kør importen igen.";
        }

        return Page();
    }
}

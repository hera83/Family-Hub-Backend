using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Recipes;

public class CategoryCreateModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty]
    public RecipeCategoryEditModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _recipesApiClient.CreateCategoryAsync(new CreateRecipeCategoryRequest
            {
                Name = Input.Name.Trim(),
                SortOrder = Input.SortOrder
            }, cancellationToken);

            TempData["SuccessMessage"] = "Opskriftskategori oprettet.";
            return RedirectToPage("/Recipes/Categories");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }
}

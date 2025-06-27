using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.CategoryDTO;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApiService _apiService;
        public CreateModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public CreateCategoryDTO Category { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var response = await _apiService.CreateCategoryAsync(Category);
            if (response?.Data != null)
            {
                return RedirectToPage("./Index");
            }
            ErrorMessage = response?.Message ?? "Failed to create category.";
            return Page();
        }
    }
} 
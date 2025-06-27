using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orchid_Razer.Services;
using Utils.DTOs.OrchidDTO;
using Utils.DTOs.CategoryDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orchids
{
    public class CreateModel : PageModel
    {
        private readonly ApiService _apiService;
        public CreateModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public CreateOrchidDTO Orchid { get; set; }
        public SelectList Categories { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }
            var response = await _apiService.CreateOrchidAsync(Orchid);
            if (response?.Data != null)
            {
                return RedirectToPage("./Index");
            }
            ErrorMessage = response?.Message ?? "Failed to create orchid.";
            await LoadCategoriesAsync();
            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var categoriesResponse = await _apiService.GetCategoriesAsync();
            if (categoriesResponse?.Data != null)
            {
                Categories = new SelectList(categoriesResponse.Data, "CategoryId", "CategoryName");
            }
        }
    }
} 
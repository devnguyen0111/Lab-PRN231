using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orchid_Razer.Services;
using Utils.DTOs.OrchidDTO;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orchids
{
    public class EditModel : PageModel
    {
        private readonly ApiService _apiService;
        public EditModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public UpdateOrchidDTO Orchid { get; set; }
        public SelectList Categories { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _apiService.GetOrchidByIdAsync(id);
            if (response?.Data == null)
            {
                return NotFound();
            }
            Orchid = new UpdateOrchidDTO
            {
                OrchidName = response.Data.OrchidName,
                OrchidDescription = response.Data.OrchidDescription,
                Price = response.Data.Price,
                IsNatural = response.Data.IsNatural,
                OrchidUrl = response.Data.OrchidUrl,
                CategoryId = response.Data.CategoryId
            };
            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }
            var response = await _apiService.UpdateOrchidAsync(id, Orchid);
            if (response?.Message == "Success")
            {
                return RedirectToPage("./Index");
            }
            ErrorMessage = response?.Message ?? "Failed to update orchid.";
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
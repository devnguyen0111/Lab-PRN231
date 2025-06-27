using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.OrchidDTO;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orchids
{
    public class DetailsModel : PageModel
    {
        private readonly ApiService _apiService;
        public DetailsModel(ApiService apiService) => _apiService = apiService;

        public OrchidDetailDTO Orchid { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _apiService.GetOrchidByIdAsync(id);
            if (response?.Data != null)
            {
                Orchid = response.Data;
                return Page();
            }
            ErrorMessage = response?.Message ?? "Orchid not found.";
            return RedirectToPage("./Index");
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.OrchidDTO;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orchids
{
    public class DeleteModel : PageModel
    {
        private readonly ApiService _apiService;
        public DeleteModel(ApiService apiService) => _apiService = apiService;

        public OrchidDetailDTO Orchid { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _apiService.GetOrchidByIdAsync(id);
            if (response?.Data == null)
            {
                return NotFound();
            }
            Orchid = response.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var response = await _apiService.DeleteOrchidAsync(id);
            if (response?.Message == "Success")
            {
                return RedirectToPage("./Index");
            }
            ErrorMessage = response?.Message ?? "Failed to delete orchid.";
            return RedirectToPage("./Index");
        }
    }
} 
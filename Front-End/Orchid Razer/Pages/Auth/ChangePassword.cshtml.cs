using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utils.DTOs.AccountDTO;
using Orchid_Razer.Services;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Auth
{
    public class ChangePasswordModel : PageModel
    {
        private readonly ApiService _apiService;
        public ChangePasswordModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public ChangePasswordDTO ChangePassword { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var response = await _apiService.ChangePasswordAsync(ChangePassword);
            if (response?.Message == "Password changed successfully")
            {
                SuccessMessage = response.Message;
                return Page();
            }
            ErrorMessage = response?.Message ?? "Password change failed.";
            return Page();
        }
    }
} 
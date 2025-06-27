using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utils.DTOs.AccountDTO;
using Orchid_Razer.Services;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ApiService _apiService;
        public ForgotPasswordModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public ForgotPasswordDTO ForgotPassword { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var response = await _apiService.ForgotPasswordAsync(ForgotPassword);
            if (response?.Message == "Password reset successful")
            {
                SuccessMessage = response.Message;
                return Page();
            }
            ErrorMessage = response?.Message ?? "Password reset failed.";
            return Page();
        }
    }
} 
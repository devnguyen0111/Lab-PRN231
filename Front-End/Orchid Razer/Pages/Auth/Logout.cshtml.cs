using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ApiService _apiService;
        public LogoutModel(ApiService apiService) => _apiService = apiService;

        public string SuccessMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _apiService.LogoutAsync();
            HttpContext.Session.Clear();
            if (response?.Message == "Logged out successfully")
            {
                SuccessMessage = response.Message;
                return RedirectToPage("/Index");
            }
            return RedirectToPage("/Index");
        }
    }
} 
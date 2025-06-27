using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utils.DTOs.AccountDTO;
using Orchid_Razer.Services;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly ApiService _apiService;
        public LoginModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public LoginDTO Login { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var response = await _apiService.LoginAsync(Login);
            if (response?.Data?.TokenAccount != null)
            {
                HttpContext.Session.SetString("Token", response.Data.TokenAccount);
                return RedirectToPage("/Index");
            }
            ErrorMessage = response?.Message ?? "Login failed.";
            return Page();
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utils.DTOs.AccountDTO;
using Orchid_Razer.Services;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly ApiService _apiService;
        public RegisterModel(ApiService apiService) => _apiService = apiService;

        [BindProperty]
        public RegisterDTO Register { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var response = await _apiService.RegisterAsync(Register);
            if (response?.Message == "Registration successful")
            {
                SuccessMessage = response.Message;
                return Page();
            }
            ErrorMessage = response?.Message ?? "Registration failed.";
            return Page();
        }
    }
} 
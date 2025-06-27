using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.CategoryDTO;
using Utils.DTOs.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;
        public IndexModel(ApiService apiService) => _apiService = apiService;

        public List<CategoryDTO> Categories { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public async Task OnGetAsync()
        {
            var response = await _apiService.GetCategoriesAsync();
            if (response?.Data != null)
            {
                Categories = response.Data;
            }
        }
    }
} 
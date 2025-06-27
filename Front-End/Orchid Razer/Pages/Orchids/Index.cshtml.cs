using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.OrchidDTO;
using Utils.DTOs.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orchids
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;
        public IndexModel(ApiService apiService) => _apiService = apiService;

        public List<OrchidListItemDTO> Orchids { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public async Task OnGetAsync()
        {
            var response = await _apiService.GetOrchidsPagedAsync(PageNumber, 10, SearchTerm);
            if (response?.Data != null)
            {
                Orchids = response.Data.Items;
                TotalPages = response.Data.TotalPages;
            }
        }
    }
} 
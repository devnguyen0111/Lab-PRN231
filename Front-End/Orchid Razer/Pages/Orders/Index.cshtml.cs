using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.OrderDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;
        public IndexModel(ApiService apiService) => _apiService = apiService;

        public List<OrderDTO> Orders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var response = await _apiService.GetOrdersAsync();
            if (response?.Data != null)
            {
                Orders = response.Data;
            }
        }
    }
} 
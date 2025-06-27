using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.OrderDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orders
{
    public class MyOrdersModel : PageModel
    {
        private readonly ApiService _apiService;
        public MyOrdersModel(ApiService apiService) => _apiService = apiService;

        public List<OrderDTO> Orders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var response = await _apiService.GetMyOrdersAsync();
            if (response?.Data != null)
            {
                Orders = response.Data;
            }
        }
    }
} 
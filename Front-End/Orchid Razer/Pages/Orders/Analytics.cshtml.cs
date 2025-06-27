using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orchid_Razer.Services;
using Utils.DTOs.OrderDTO;
using System;
using System.Threading.Tasks;

namespace Orchid_Razer.Pages.Orders
{
    public class AnalyticsModel : PageModel
    {
        private readonly ApiService _apiService;
        public AnalyticsModel(ApiService apiService) => _apiService = apiService;

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        public OrderAnalyticsDTO Analytics { get; set; }

        public async Task OnGetAsync()
        {
            var response = await _apiService.GetOrderAnalyticsAsync(StartDate, EndDate);
            if (response?.Data != null)
            {
                Analytics = response.Data;
            }
        }
    }
} 
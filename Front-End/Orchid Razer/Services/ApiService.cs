using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Linq;
using Utils.DTOs.AccountDTO;
using Utils.DTOs.CategoryDTO;
using Utils.DTOs.OrchidDTO;
using Utils.DTOs.OrderDTO;
using Utils.DTOs.ApiResponse;
using Utils.Paginated;

namespace Orchid_Razer.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl = "https://your-api-base-url.com"; // TODO: Set your API base URL
        private static readonly MediaTypeHeaderValue _jsonMediaType = new("application/json");

        public ApiService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient(bool authorized = false)
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new System.Uri(_baseUrl);
            if (authorized)
            {
                var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            return client;
        }

        // Example: Login
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginDTO login)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("/api/Account/login", login);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Orchid paged list
        public async Task<ApiResponse<PaginatedList<OrchidListItemDTO>>> GetOrchidsPagedAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var client = CreateClient();
            var url = $"/api/Orchid/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"&searchTerm={System.Net.WebUtility.UrlEncode(searchTerm)}";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<PaginatedList<OrchidListItemDTO>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Register
        public async Task<ApiResponse<object>> RegisterAsync(RegisterDTO register)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("/api/Account/register", register);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Forgot Password
        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDTO forgotPassword)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("/api/Account/forgot-password", forgotPassword);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Change Password
        public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDTO changePassword)
        {
            var client = CreateClient(true);
            var response = await client.PostAsJsonAsync("/api/Account/change-password", changePassword);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Logout
        public async Task<ApiResponse<object>> LogoutAsync()
        {
            var client = CreateClient(true);
            var response = await client.PostAsync("/api/Account/logout", null);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Orchid CRUD operations
        public async Task<ApiResponse<OrchidDetailDTO>> GetOrchidByIdAsync(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/Orchid/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<OrchidDetailDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> CreateOrchidAsync(CreateOrchidDTO orchid)
        {
            var client = CreateClient(true);
            var response = await client.PostAsJsonAsync("/api/Orchid", orchid);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> UpdateOrchidAsync(int id, UpdateOrchidDTO orchid)
        {
            var client = CreateClient(true);
            var response = await client.PutAsJsonAsync($"/api/Orchid/{id}", orchid);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> DeleteOrchidAsync(int id)
        {
            var client = CreateClient(true);
            var response = await client.DeleteAsync($"/api/Orchid/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Orchid filtering and search
        public async Task<ApiResponse<PaginatedList<OrchidListItemDTO>>> FilterOrchidsAsync(string searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, bool? isNatural = null, int? categoryId = null, string sortBy = null, bool ascending = true, int pageNumber = 1, int pageSize = 10)
        {
            var client = CreateClient();
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={System.Net.WebUtility.UrlEncode(searchTerm)}");
            if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice}");
            if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice}");
            if (isNatural.HasValue) queryParams.Add($"isNatural={isNatural}");
            if (categoryId.HasValue) queryParams.Add($"categoryId={categoryId}");
            if (!string.IsNullOrEmpty(sortBy)) queryParams.Add($"sortBy={sortBy}");
            queryParams.Add($"ascending={ascending}");
            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");

            var url = $"/api/Orchid/filter?{string.Join("&", queryParams)}";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<PaginatedList<OrchidListItemDTO>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Category operations
        public async Task<ApiResponse<List<CategoryDTO>>> GetCategoriesAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/api/Category");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<CategoryDTO>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/Category/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<CategoryDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> CreateCategoryAsync(CreateCategoryDTO category)
        {
            var client = CreateClient(true);
            var response = await client.PostAsJsonAsync("/api/Category", category);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> UpdateCategoryAsync(int id, UpdateCategoryDTO category)
        {
            var client = CreateClient(true);
            var response = await client.PutAsJsonAsync($"/api/Category/{id}", category);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> DeleteCategoryAsync(int id)
        {
            var client = CreateClient(true);
            var response = await client.DeleteAsync($"/api/Category/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Order operations
        public async Task<ApiResponse<List<OrderDTO>>> GetOrdersAsync()
        {
            var client = CreateClient(true);
            var response = await client.GetAsync("/api/Order");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<OrderDTO>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<OrderDTO>> GetOrderByIdAsync(int id)
        {
            var client = CreateClient(true);
            var response = await client.GetAsync($"/api/Order/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<OrderDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<List<OrderDTO>>> GetMyOrdersAsync()
        {
            var client = CreateClient(true);
            var response = await client.GetAsync("/api/Order/my-orders");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<OrderDTO>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> CreateOrderAsync(CreateOrderDTO order)
        {
            var client = CreateClient(true);
            var response = await client.PostAsJsonAsync("/api/Order", order);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO status)
        {
            var client = CreateClient(true);
            var response = await client.PutAsJsonAsync($"/api/Order/{id}/status", status);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> CancelOrderAsync(int id)
        {
            var client = CreateClient(true);
            var response = await client.PostAsync($"/api/Order/{id}/cancel", null);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<OrderAnalyticsDTO>> GetOrderAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var client = CreateClient(true);
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var url = $"/api/Order/statistics";
            if (queryParams.Any()) url += $"?{string.Join("&", queryParams)}";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<OrderAnalyticsDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
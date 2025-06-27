using Models.Models;
using Repositorys.DTOs.OrderDTO;
using Repositorys.PaginatedList;

namespace Services.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<PaginatedList<Order>> GetOrdersPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Order>> GetOrdersByAccountAsync(int accountId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<Order> CreateOrderAsync(int accountId, CreateOrderDTO orderDTO);
        Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO statusDTO);
        Task<bool> CancelOrderAsync(int id);
        Task<decimal> CalculateOrderTotalAsync(List<OrderItemDTO> items);
        Task<OrderAnalyticsDTO> GetAnalyticsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetRevenueByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetOrderStatusDistributionAsync();
        Task<List<TopSellingOrchidDTO>> GetTopSellingOrchidsAsync(int count = 5);
    }
}
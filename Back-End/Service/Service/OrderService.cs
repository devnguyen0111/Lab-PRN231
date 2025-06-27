using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositorys.DTOs.OrderDTO;
using Repositorys.IRepositories;
using Repositorys.PaginatedList;
using Services.IService;

namespace Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUOW _unitOfWork;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<Orchid> _orchidRepository;

        public OrderService(IUOW unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = _unitOfWork.GetRepository<Order>();
            _orderDetailRepository = _unitOfWork.GetRepository<OrderDetail>();
            _orchidRepository = _unitOfWork.GetRepository<Orchid>();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.Entities
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.Entities
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<PaginatedList<Order>> GetOrdersPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _orderRepository.Entities
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .OrderByDescending(o => o.OrderDate);

            return await _orderRepository.GetPagging(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Order>> GetOrdersByAccountAsync(int accountId)
        {
            return await _orderRepository.Entities
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .Where(o => o.AccountId == accountId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _orderRepository.Entities
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(int accountId, CreateOrderDTO orderDTO)
        {
            // Calculate total and validate items
            var totalAmount = await CalculateOrderTotalAsync(orderDTO.OrderItems);

            var order = new Order
            {
                Id = await GenerateNewOrderId(),
                AccountId = accountId,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderStatus = "Pending",
                TotalAmount = totalAmount
            };

            await _orderRepository.InsertAsync(order);

            foreach (var item in orderDTO.OrderItems)
            {
                var orchid = await _orchidRepository.GetByIdAsync(item.OrchidId)
                    ?? throw new InvalidOperationException($"Orchid with ID {item.OrchidId} not found");

                var orderDetail = new OrderDetail
                {
                    Id = await GenerateNewOrderDetailId(),
                    OrderId = order.Id,
                    OrchidId = item.OrchidId,
                    Quantity = item.Quantity,
                    Price = orchid.Price
                };

                await _orderDetailRepository.InsertAsync(orderDetail);
            }

            await _unitOfWork.SaveAsync();
            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO statusDTO)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            if (order.OrderStatus == "Cancelled")
                throw new InvalidOperationException("Cannot update status of cancelled order");

            order.OrderStatus = statusDTO.OrderStatus;
            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            if (order.OrderStatus != "Pending")
                throw new InvalidOperationException("Can only cancel pending orders");

            order.OrderStatus = "Cancelled";
            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<decimal> CalculateOrderTotalAsync(List<OrderItemDTO> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                var orchid = await _orchidRepository.GetByIdAsync(item.OrchidId)
                    ?? throw new InvalidOperationException($"Orchid with ID {item.OrchidId} not found");

                total += orchid.Price * item.Quantity;
            }
            return total;
        }

        private async Task<int> GenerateNewOrderId()
        {
            var maxId = await _orderRepository.Entities
                .MaxAsync(o => (int?)o.Id) ?? 0;
            return maxId + 1;
        }

        private async Task<int> GenerateNewOrderDetailId()
        {
            var maxId = await _orderDetailRepository.Entities
                .MaxAsync(od => (int?)od.Id) ?? 0;
            return maxId + 1;
        }

        public async Task<OrderAnalyticsDTO> GetAnalyticsAsync(DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var orders = await _orderRepository.Entities
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                        .ThenInclude(o => o.Category)
                .Where(o => o.OrderDate >= startDateOnly && o.OrderDate <= endDateOnly)
                .ToListAsync();

            var analytics = new OrderAnalyticsDTO
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
                OrdersByStatus = orders.GroupBy(o => o.OrderStatus)
                    .ToDictionary(g => g.Key, g => g.Count()),
                RevenueByCategory = orders.SelectMany(o => o.OrderDetails)
                    .GroupBy(od => od.Orchid.Category.CategoryName)
                    .ToDictionary(g => g.Key, g => g.Sum(od => od.Price * od.Quantity)),
                DailyRevenue = orders.GroupBy(o => o.OrderDate)
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount))
            };

            analytics.TopSellingOrchids = await GetTopSellingOrchidsAsync();

            return analytics;
        }

        public async Task<List<TopSellingOrchidDTO>> GetTopSellingOrchidsAsync(int count = 5)
        {
            return await _orderDetailRepository.Entities
                .Include(od => od.Orchid)
                    .ThenInclude(o => o.Category)
                .GroupBy(od => new { od.Orchid.OrchidId, od.Orchid.OrchidName, od.Orchid.Category.CategoryName })
                .Select(g => new TopSellingOrchidDTO
                {
                    OrchidId = g.Key.OrchidId,
                    OrchidName = g.Key.OrchidName,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Price * od.Quantity)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            return await _orderRepository.Entities
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .Where(o => o.OrderDate >= startDateOnly && o.OrderDate <= endDateOnly)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<decimal> GetRevenueByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            return await _orderRepository.Entities
                .Where(o => o.OrderDate >= startDateOnly && o.OrderDate <= endDateOnly)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<Dictionary<string, int>> GetOrderStatusDistributionAsync()
        {
            return await _orderRepository.Entities
                .GroupBy(o => o.OrderStatus)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}
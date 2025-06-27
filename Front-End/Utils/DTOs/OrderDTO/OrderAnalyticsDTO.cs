namespace Utils.DTOs.OrderDTO
{
    public class OrderAnalyticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
        public Dictionary<string, decimal> RevenueByCategory { get; set; } = new();
        public Dictionary<DateOnly, decimal> DailyRevenue { get; set; } = new();
        public List<TopSellingOrchidDTO> TopSellingOrchids { get; set; } = new();
    }
}
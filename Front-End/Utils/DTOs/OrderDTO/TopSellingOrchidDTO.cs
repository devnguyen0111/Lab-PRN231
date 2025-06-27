namespace Utils.DTOs.OrderDTO
{
    public class TopSellingOrchidDTO
    {
        public int OrchidId { get; set; }
        public string OrchidName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
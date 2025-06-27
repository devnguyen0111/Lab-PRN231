namespace Utils.DTOs.OrderDTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();
    }

    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int? OrchidId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string OrchidName { get; set; } = string.Empty;
    }
} 
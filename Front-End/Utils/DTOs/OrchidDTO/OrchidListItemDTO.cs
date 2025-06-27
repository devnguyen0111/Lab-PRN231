namespace Utils.DTOs.OrchidDTO
{
    public class OrchidListItemDTO
    {
        public int OrchidId { get; set; }
        public string OrchidName { get; set; } = string.Empty;
        public string? OrchidDescription { get; set; }
        public decimal Price { get; set; }
        public bool IsNatural { get; set; }
        public string? OrchidUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
} 
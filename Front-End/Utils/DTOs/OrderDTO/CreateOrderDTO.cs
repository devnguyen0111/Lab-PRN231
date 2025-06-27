using System.ComponentModel.DataAnnotations;

namespace Utils.DTOs.OrderDTO
{
    public class CreateOrderDTO
    {
        [Required]
        public required List<OrderItemDTO> OrderItems { get; set; }
    }

    public class OrderItemDTO
    {
        [Required]
        public required int OrchidId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public required int Quantity { get; set; }
    }
}
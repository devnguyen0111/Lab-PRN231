using System.ComponentModel.DataAnnotations;

namespace Utils.DTOs.OrderDTO
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        [StringLength(20)]
        public required string OrderStatus { get; set; }
    }
}
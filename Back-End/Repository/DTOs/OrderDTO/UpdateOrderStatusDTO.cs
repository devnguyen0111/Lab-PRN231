using System.ComponentModel.DataAnnotations;

namespace Repositorys.DTOs.OrderDTO
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        [StringLength(20)]
        public required string OrderStatus { get; set; }
    }
}
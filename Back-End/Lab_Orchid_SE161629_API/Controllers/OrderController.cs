using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositorys.DTOs.OrderDTO;
using Repositorys.PaginatedList;
using Services.IService;

namespace Lab_Orchid_SE161629_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAccountService _accountService;

        public OrderController(IOrderService orderService, IAccountService accountService)
        {
            _orderService = orderService;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Order>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(BaseResponseModel<IEnumerable<Order>>.OkResponseModel(orders));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseModel<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<Order>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(BaseResponseModel<Order>.NotFoundResponseModel(null, new { message = "Order not found" }));

            return Ok(BaseResponseModel<Order>.OkResponseModel(order));
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(BaseResponseModel<PaginatedList<Order>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdersPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersPaginatedAsync(pageNumber, pageSize);
            return Ok(BaseResponseModel<PaginatedList<Order>>.OkResponseModel(orders));
        }

        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Order>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = await _accountService.GetCurrentUserIdAsync();
            var orders = await _orderService.GetOrdersByAccountAsync(int.Parse(userId));
            return Ok(BaseResponseModel<IEnumerable<Order>>.OkResponseModel(orders));
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Order>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(BaseResponseModel<IEnumerable<Order>>.OkResponseModel(orders));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseModel<Order>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseModel<Order>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDTO)
        {
            try
            {
                var userId = await _accountService.GetCurrentUserIdAsync();
                var order = await _orderService.CreateOrderAsync(int.Parse(userId), orderDTO);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id },
                    BaseResponseModel<Order>.OkResponseModel(order));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<Order>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO statusDTO)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, statusDTO);
                if (!result)
                    return NotFound(BaseResponseModel<string>.NotFoundResponseModel(null, new { message = "Order not found" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(string.Empty, new { message = "Order status updated successfully" }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                if (!result)
                    return NotFound(BaseResponseModel<string>.NotFoundResponseModel(null, new { message = "Order not found" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(string.Empty, new { message = "Order cancelled successfully" }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(BaseResponseModel<OrderAnalyticsDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var analytics = await _orderService.GetAnalyticsAsync(startDate ?? DateTime.Today.AddMonths(-1), endDate ?? DateTime.Today);
            return Ok(BaseResponseModel<OrderAnalyticsDTO>.OkResponseModel(analytics));
        }
    }
}
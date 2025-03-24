using Microsoft.AspNetCore.Mvc;
using SalesApp.DTOs;
using SalesApp.Models;
using SalesApp.Services.Interfaces;
using static SalesApp.DTOs.OrderDTO;

namespace SalesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> ListOrdersWithFilter([FromQuery] string status = null)
        {
            IEnumerable<OrderDTO> orders;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                orders = await _orderService.GetOrdersByStatusAsync(orderStatus);
            }
            else
            {
                orders = await _orderService.GetAllOrdersAsync();
            }
            return Ok(orders);
        }
        [HttpGet("details/{orderId}")]
        public async Task<ActionResult<OrderDTO>> FindOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();
            return Ok(order);
        }
        [HttpPost("create")]
        public async Task<ActionResult<int>> PlaceNewOrder(CreateOrderDTO orderDto)
        {
            try
            {
                var orderId = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(FindOrderById), new { orderId }, orderId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromBody] OrderStatusUpdateDTO statusUpdate)
        {
            try
            {
                if (!Enum.TryParse<OrderStatus>(statusUpdate.Status, true, out var orderStatus))
                {
                    return BadRequest("Status inválido");
                }
                await _orderService.UpdateOrderStatusAsync(orderId, orderStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {
            var userId = User.GetUserId();
            var orders = await _orderService.GetOrdersByUserIdAsync(userId, orderParams);
            Response.AddPaginationHeader(orders.Metadata);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var userId = User.GetUserId();
            var order = await _orderService.GetOrderByIdAsync(userId, id);
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] OrderRequest orderRequest)
        {
            var userId = User.GetUserId();
            var createdOrder = await _orderService.CreateOrderAsync(userId, orderRequest);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromForm] OrderStatusRequest orderStatusRequest)
        {
            var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, orderStatusRequest);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userId = User.GetUserId();
            await _orderService.CancelOrderAsync(userId, id);
            return NoContent();
        }
    }
}
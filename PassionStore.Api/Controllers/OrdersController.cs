using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {
            var orders = await _orderService.GetOrdersAsync(orderParams);
            Response.AddPaginationHeader(orders.Metadata);
            return Ok(orders);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetSelfOrders([FromQuery] OrderParams orderParams)
        {
            var userId = User.GetUserId();
            var orders = await _orderService.GetSelfOrdersAsync(userId, orderParams);
            Response.AddPaginationHeader(orders.Metadata);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var userId = User.GetUserId();
            var order = await _orderService.GetOrderByIdAsync(userId, id);
            return Ok(order);
        }

        [HttpPost]
        [Authorize]
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

        [HttpPost("{id}/return")]
        [Authorize]
        public async Task<IActionResult> RequestReturn(Guid id, [FromForm] ReturnRequest returnRequest)
        {
            var userId = User.GetUserId();
            var order = await _orderService.RequestReturnAsync(userId, id, returnRequest);
            return Ok(order);
        }

        [HttpPut("{id}/return/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReturnStatus(Guid id, [FromForm] ReturnStatusRequest returnStatusRequest)
        {
            var order = await _orderService.UpdateReturnStatusAsync(id, returnStatusRequest);
            return Ok(order);
        }

        [HttpDelete("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid id, string? cancellationReason)
        {
            var userId = User.GetUserId();
            await _orderService.CancelOrderAsync(userId, id, cancellationReason);
            return NoContent();
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> HandlePaymentCallback(
            [FromQuery] string code,
            [FromQuery] string id,
            [FromQuery] bool cancel,
            [FromQuery] string status,
            [FromQuery] long orderCode)
        {
            var orderResponse = await _orderService.HandlePaymentCallbackAsync(code, id, cancel, status, orderCode);
            if (orderResponse != null)
                return Ok(orderResponse);
            return NoContent();
        }
    }
}
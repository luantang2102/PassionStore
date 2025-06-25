using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Extensions;

namespace PassionStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductVariantRepository productVariantRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productVariantRepository = productVariantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || !order.UserProfile.UserId.Equals(userId))
            {
                var attributes = new Dictionary<string, object> { { "orderId", orderId } };
                throw new AppException(ErrorCode.ORDER_NOT_FOUND, attributes);
            }
            return order.MapModelToResponse();
        }

        public async Task<PagedList<OrderResponse>> GetOrdersByUserIdAsync(Guid userId, OrderParams orderParams)
        {
            var query = _orderRepository.GetByUserIdAsync(userId)
                .Sort(orderParams.OrderBy)
                .Filter(orderParams.Status);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(projectedQuery, orderParams.PageNumber, orderParams.PageSize);
        }

        public async Task<OrderResponse> CreateOrderAsync(Guid userId, OrderRequest orderRequest)
        {
            var userProfile = await _orderRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfile == null)
            {
                var attributes = new Dictionary<string, object> { { "userId", userId } };
                throw new AppException(ErrorCode.USER_PROFILE_NOT_FOUND, attributes);
            }

            
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                var attributes = new Dictionary<string, object> { { "userId", userId } };
                throw new AppException(ErrorCode.CART_NOT_FOUND, attributes);
            }

            var shippingAddress = String.Concat(
                userProfile.SpecificAddress ?? string.Empty, ", ",
                userProfile.Ward ?? string.Empty, ", ",
                userProfile.District ?? string.Empty, ", ",
                userProfile.Province ?? string.Empty
            );

            var order = new Order
            {
                UserProfileId = userProfile.Id,
                ShippingAddress = shippingAddress,
                PaymentMethod = orderRequest.PaymentMethod,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0,
                OrderItems = []
            };

            decimal totalAmount = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var productVariant = await _productVariantRepository.GetByIdAsync(cartItem.ProductVariantId);
                if (productVariant == null)
                {
                    var attributes = new Dictionary<string, object> { { "productVariantId", cartItem.ProductVariantId } };
                    throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
                }

                if (productVariant.StockQuantity < cartItem.Quantity)
                {
                    var attributes = new Dictionary<string, object> { { "productVariantId", cartItem.ProductVariantId } };
                    throw new AppException(ErrorCode.INSUFFICIENT_STOCK, attributes);
                }

                var orderItem = new OrderItem
                {
                    ProductVariantId = cartItem.ProductVariantId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                    Order = order
                };
                order.OrderItems.Add(orderItem);
                totalAmount += cartItem.Price * cartItem.Quantity;

                productVariant.StockQuantity -= cartItem.Quantity;
                await _productVariantRepository.UpdateAsync(productVariant);
            }

            order.TotalAmount = totalAmount;

            var createdOrder = await _orderRepository.CreateAsync(order);
            cart.CartItems.Clear();
            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CommitAsync();
            return createdOrder.MapModelToResponse();
        }

        public async Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequest orderStatusRequest)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                var attributes = new Dictionary<string, object> { { "orderId", orderId } };
                throw new AppException(ErrorCode.ORDER_NOT_FOUND, attributes);
            }

            order.Status = orderStatusRequest.Status;
            order.UpdatedDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();
            return order.MapModelToResponse();
        }

        public async Task CancelOrderAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || !order.UserProfile.UserId.Equals(userId))
            {
                var attributes = new Dictionary<string, object> { { "orderId", orderId } };
                throw new AppException(ErrorCode.ORDER_NOT_FOUND, attributes);
            }

            if (order.Status != "Pending")
            {
                var attributes = new Dictionary<string, object> { { "orderId", orderId } };
                throw new AppException(ErrorCode.ORDER_NOT_CANCELLABLE, attributes);
            }

            foreach (var orderItem in order.OrderItems)
            {
                var productVariant = await _productVariantRepository.GetByIdAsync(orderItem.ProductVariantId);
                if (productVariant != null)
                {
                    productVariant.StockQuantity += orderItem.Quantity;
                    await _productVariantRepository.UpdateAsync(productVariant);
                }
            }

            await _orderRepository.DeleteAsync(order);
            await _unitOfWork.CommitAsync();
        }
    }
}
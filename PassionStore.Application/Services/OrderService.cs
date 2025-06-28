using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Externals.Payos;
using PassionStore.Infrastructure.Externals.Payos.Models;
using Net.payOS.Types;
using PassionStore.Infrastructure.Extensions;
using static PassionStore.Infrastructure.Externals.Payos.PayOSService;
using PassionStore.Core.Enums;
using PassionStore.Infrastructure.Repositories;

namespace PassionStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOSService _payOSService;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductVariantRepository productVariantRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            PayOSService payOSService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productVariantRepository = productVariantRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _payOSService = payOSService;
        }

        public async Task<PagedList<OrderResponse>> GetOrdersAsync(OrderParams orderParams)
        {
            var query = _orderRepository.GetAllAsync()
                .Sort(orderParams.OrderBy)
                .Search(orderParams.SearchTerm)
                .Filter(orderParams.Status, orderParams.UserId);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(
                projectedQuery,
                orderParams.PageNumber,
                orderParams.PageSize
            );
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new AppException(ErrorCode.ORDER_NOT_FOUND);
            }

            if (!await _userRepository.IsInRoleAsync(user, "Admin") && !order.UserProfile.UserId.Equals(userId))
            {
                throw new AppException(ErrorCode.ACCESS_DENIED);
            }

            return order.MapModelToResponse();
        }

        public async Task<PagedList<OrderResponse>> GetSelfOrdersAsync(Guid userId, OrderParams orderParams)
        {
            var query = _orderRepository.GetByUserIdAsync(userId)
                .Sort(orderParams.OrderBy)
                .Filter(orderParams.Status, orderParams.UserId);

            var projectedQuery = query.Select(x => x.MapModelToResponse());
            return await PaginationService.ToPagedList(projectedQuery, orderParams.PageNumber, orderParams.PageSize);
        }

        public async Task<OrderResponse> CreateOrderAsync(Guid userId, OrderRequest orderRequest)
        {
            var userProfile = await _orderRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfile == null)
            {
                throw new AppException(ErrorCode.USER_PROFILE_NOT_FOUND);
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new AppException(ErrorCode.CART_NOT_FOUND);
            }

            try
            {
                var shippingAddress = string.Concat(
                    userProfile.SpecificAddress ?? string.Empty, ", ",
                    userProfile.Ward ?? string.Empty, ", ",
                    userProfile.District ?? string.Empty, ", ",
                    userProfile.Province ?? string.Empty).TrimEnd(',', ' ');

                var order = new Order
                {
                    UserProfileId = userProfile.Id,
                    ShippingAddress = shippingAddress,
                    PaymentMethod = orderRequest.PaymentMethod,
                    ShippingMethod = orderRequest.ShippingMethod,
                    Note = orderRequest.Note ?? string.Empty,
                    Status = orderRequest.PaymentMethod == PaymentMethod.PayOS ? OrderStatus.PendingPayment : OrderStatus.OrderConfirmed,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0,
                    OrderItems = []
                };

                decimal totalAmount = 0;
                var payOSItems = new List<ItemData>();
                foreach (var cartItem in cart.CartItems)
                {
                    var productVariant = await _productVariantRepository.GetByIdAsync(cartItem.ProductVariantId);
                    if (productVariant == null)
                    {
                        throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND);
                    }

                    if (productVariant.StockQuantity < cartItem.Quantity)
                    {
                        throw new AppException(ErrorCode.INSUFFICIENT_STOCK);
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

                    payOSItems.Add(new ItemData(
                        name: productVariant.Product.Name,
                        quantity: cartItem.Quantity,
                        price: (int)cartItem.Price
                    ));

                    productVariant.StockQuantity -= cartItem.Quantity;
                    await _productVariantRepository.UpdateAsync(productVariant);
                }

                // Add shipping cost to total amount
                totalAmount += (int)orderRequest.ShippingMethod;
                order.TotalAmount = totalAmount;

                await _orderRepository.CreateAsync(order);
                await _unitOfWork.CommitAsync();

                cart.CartItems.Clear();
                await _cartRepository.UpdateAsync(cart);

                if (orderRequest.PaymentMethod == PaymentMethod.PayOS)
                {
                    try
                    {
                        var payOSRequest = new PayOSRequest(
                            Amount: (int)totalAmount, // Include shipping cost
                            Description: $"{orderRequest.Note}",
                            OrderCode: (int)(DateTime.UtcNow.Ticks % 1000000),
                            Items: payOSItems
                        );

                        var payOSResponse = await _payOSService.CreatePaymentAsync(payOSRequest);

                        order.PaymentLink = payOSResponse.CheckoutUrl;
                        order.PaymentTransactionId = payOSResponse.OrderCode.ToString();
                        order.Status = OrderStatus.PendingPayment; // Explicitly set for clarity
                        await _orderRepository.UpdateAsync(order);
                    }
                    catch (PayOSException)
                    {
                        throw new AppException(ErrorCode.PAYMENT_CREATION_FAILED);
                    }
                }

                await _unitOfWork.CommitAsync();

                return order.MapModelToResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequest orderStatusRequest)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new AppException(ErrorCode.ORDER_NOT_FOUND);
                }

                // Validate status transition
                if (!IsValidStatusTransition(order.Status, orderStatusRequest.Status))
                {
                    throw new AppException(ErrorCode.INVALID_STATUS_TRANSITION);
                }

                order.Status = orderStatusRequest.Status;
                order.UpdatedDate = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.CommitAsync();

                return order.MapModelToResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CancelOrderAsync(Guid userId, Guid orderId, string? cancellationReason, bool callBack = false)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || !order.UserProfile.UserId.Equals(userId))
                {
                    throw new AppException(ErrorCode.ORDER_NOT_FOUND);
                }

                if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.OrderConfirmed)
                {
                    throw new AppException(ErrorCode.ORDER_NOT_CANCELLABLE);
                }

                foreach (var orderItem in order.OrderItems)
                {
                    var productVariant = await _productVariantRepository.GetByIdAsync(orderItem.ProductVariantId);
                    if (productVariant == null)
                    {
                        throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND);
                    }

                    productVariant.StockQuantity += orderItem.Quantity;
                    await _productVariantRepository.UpdateAsync(productVariant);
                }

                if (!callBack && order.PaymentMethod == PaymentMethod.PayOS && !string.IsNullOrEmpty(order.PaymentTransactionId))
                {
                    if (!long.TryParse(order.PaymentTransactionId, out var orderCode))
                    {
                        throw new AppException(ErrorCode.PAYMENT_CANCELLATION_FAILED);
                    }

                    try
                    {
                        await _payOSService.CancelOrder(orderCode, cancellationReason ?? "Order cancelled by user");
                    }
                    catch (PayOSException)
                    {
                        foreach (var orderItem in order.OrderItems)
                        {
                            var productVariant = await _productVariantRepository.GetByIdAsync(orderItem.ProductVariantId);
                            if (productVariant != null)
                            {
                                productVariant.StockQuantity -= orderItem.Quantity;
                                await _productVariantRepository.UpdateAsync(productVariant);
                            }
                        }
                        throw new AppException(ErrorCode.PAYMENT_CANCELLATION_FAILED);
                    }
                }

                order.Status = OrderStatus.Cancelled;
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderResponse?> HandlePaymentCallbackAsync(string code, string id, bool cancel, string status, long orderCode)
        {
            try
            {
                var paymentInfo = await _payOSService.PaymentCallBack(code, id, cancel, status, orderCode);
                if (paymentInfo != null)
                {
                    var order = await _orderRepository.GetByPaymentTransactionIdAsync(paymentInfo.OrderCode.ToString());
                    if (order == null)
                    {
                        throw new AppException(ErrorCode.ORDER_NOT_FOUND);
                    }

                    var orderStatusRequest = new OrderStatusRequest
                    {
                        Status = paymentInfo.Status == "PAID" ? OrderStatus.PaymentConfirmed : OrderStatus.PaymentFailed
                    };
                    return await UpdateOrderStatusAsync(order.Id, orderStatusRequest);
                }

                if (cancel && status == "CANCELLED" && !string.IsNullOrEmpty(orderCode.ToString()))
                {
                    var order = await _orderRepository.GetByPaymentTransactionIdAsync(orderCode.ToString());
                    if (order == null)
                    {
                        throw new AppException(ErrorCode.ORDER_NOT_FOUND);
                    }

                    await CancelOrderAsync(order.UserProfile.UserId, order.Id, "Cancelled via PayOS callback", true);
                    return null;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (OrderStatus.PendingPayment, OrderStatus.PaymentConfirmed) => true,
                (OrderStatus.PendingPayment, OrderStatus.PaymentFailed) => true,
                (OrderStatus.PendingPayment, OrderStatus.OrderConfirmed) => true,
                (OrderStatus.PendingPayment, OrderStatus.OnHold) => true,
                (OrderStatus.PendingPayment, OrderStatus.Cancelled) => true,
                (OrderStatus.PaymentConfirmed, OrderStatus.OrderConfirmed) => true,
                (OrderStatus.PaymentConfirmed, OrderStatus.OnHold) => true,
                (OrderStatus.PaymentConfirmed, OrderStatus.Cancelled) => true,
                (OrderStatus.OrderConfirmed, OrderStatus.Processing) => true,
                (OrderStatus.OrderConfirmed, OrderStatus.OnHold) => true,
                (OrderStatus.OrderConfirmed, OrderStatus.Cancelled) => true,
                (OrderStatus.Processing, OrderStatus.ReadyToShip) => true,
                (OrderStatus.Processing, OrderStatus.OnHold) => true,
                (OrderStatus.Processing, OrderStatus.Cancelled) => true,
                (OrderStatus.ReadyToShip, OrderStatus.Shipped) => true,
                (OrderStatus.ReadyToShip, OrderStatus.OnHold) => true,
                (OrderStatus.ReadyToShip, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipped, OrderStatus.OutForDelivery) => true,
                (OrderStatus.Shipped, OrderStatus.OnHold) => true,
                (OrderStatus.OutForDelivery, OrderStatus.Delivered) => true,
                (OrderStatus.OutForDelivery, OrderStatus.Returned) => true,
                (OrderStatus.Delivered, OrderStatus.PaymentReceived) => true,
                (OrderStatus.Delivered, OrderStatus.Returned) => true,
                (OrderStatus.PaymentReceived, OrderStatus.Completed) => true,
                (OrderStatus.Returned, OrderStatus.Refunded) => true,
                (OrderStatus.Returned, OrderStatus.Completed) => true,
                _ => false
            };
        }
    }
}
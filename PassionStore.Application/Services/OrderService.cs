using Net.payOS.Types;
using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Enums;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Extensions;
using PassionStore.Infrastructure.Externals.Payos;
using static PassionStore.Infrastructure.Externals.Payos.PayOSService;

namespace PassionStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOSService _payOSService;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductVariantRepository productVariantRepository,
            IUserRepository userRepository,
            IRatingRepository ratingRepository,
            IUnitOfWork unitOfWork,
            PayOSService payOSService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productVariantRepository = productVariantRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            _unitOfWork = unitOfWork;
            _payOSService = payOSService;
        }

        private readonly Dictionary<PaymentMethod, Dictionary<OrderStatus, List<OrderStatus>>> _validTransitions = new()
        {
            {
                PaymentMethod.PayOS, new Dictionary<OrderStatus, List<OrderStatus>>
                {
                    { OrderStatus.PendingPayment, new List<OrderStatus> { OrderStatus.PaymentConfirmed, OrderStatus.PaymentFailed, OrderStatus.Cancelled } },
                    { OrderStatus.PaymentConfirmed, new List<OrderStatus> { OrderStatus.OrderConfirmed, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.PaymentFailed, new List<OrderStatus> { OrderStatus.PendingPayment, OrderStatus.Cancelled } },
                    { OrderStatus.OrderConfirmed, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.ReadyToShip, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.ReadyToShip, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.OutForDelivery, OrderStatus.OnHold } },
                    { OrderStatus.OutForDelivery, new List<OrderStatus> { OrderStatus.Delivered } },
                    { OrderStatus.Delivered, new List<OrderStatus> { OrderStatus.ReturnRequested, OrderStatus.Completed } },
                    { OrderStatus.ReturnRequested, new List<OrderStatus> { OrderStatus.Returned, OrderStatus.Completed } },
                    { OrderStatus.Returned, new List<OrderStatus> { OrderStatus.Refunded, OrderStatus.Completed } },
                    { OrderStatus.Refunded, new List<OrderStatus> { OrderStatus.Completed } },
                    { OrderStatus.OnHold, new List<OrderStatus> { OrderStatus.OrderConfirmed } },
                    { OrderStatus.Cancelled, new List<OrderStatus>() }
                }
            },
            {
                PaymentMethod.COD, new Dictionary<OrderStatus, List<OrderStatus>>
                {
                    { OrderStatus.OrderConfirmed, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.ReadyToShip, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.ReadyToShip, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.OnHold, OrderStatus.Cancelled } },
                    { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.OutForDelivery, OrderStatus.OnHold } },
                    { OrderStatus.OutForDelivery, new List<OrderStatus> { OrderStatus.Delivered } },
                    { OrderStatus.Delivered, new List<OrderStatus> { OrderStatus.PaymentReceived, OrderStatus.ReturnRequested } },
                    { OrderStatus.PaymentReceived, new List<OrderStatus> { OrderStatus.Completed } },
                    { OrderStatus.ReturnRequested, new List<OrderStatus> { OrderStatus.Returned, OrderStatus.Completed } },
                    { OrderStatus.Returned, new List<OrderStatus> { OrderStatus.Refunded, OrderStatus.Completed } },
                    { OrderStatus.Refunded, new List<OrderStatus> { OrderStatus.Completed } },
                    { OrderStatus.OnHold, new List<OrderStatus> { OrderStatus.OrderConfirmed } },
                    { OrderStatus.Cancelled, new List<OrderStatus>() }
                }
            }
        };

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
                throw new AppException(ErrorCode.USER_NOT_FOUND);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new AppException(ErrorCode.ORDER_NOT_FOUND);

            if (!await _userRepository.IsInRoleAsync(user, "Admin") && !order.UserProfile.UserId.Equals(userId))
                throw new AppException(ErrorCode.ACCESS_DENIED);

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
                throw new AppException(ErrorCode.USER_PROFILE_NOT_FOUND);

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new AppException(ErrorCode.CART_NOT_FOUND);

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
                    OrderItems = [],
                    CreatedDate = DateTime.UtcNow
                };

                decimal totalAmount = 0;
                var payOSItems = new List<ItemData>();
                foreach (var cartItem in cart.CartItems)
                {
                    var productVariant = await _productVariantRepository.GetByIdAsync(cartItem.ProductVariantId);
                    if (productVariant == null)
                        throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND);

                    if (productVariant.StockQuantity < cartItem.Quantity)
                        throw new AppException(ErrorCode.INSUFFICIENT_STOCK);

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
                            Amount: (int)totalAmount,
                            Description: $"{orderRequest.Note}",
                            OrderCode: (int)(DateTime.UtcNow.Ticks % 1000000),
                            Items: payOSItems
                        );

                        var payOSResponse = await _payOSService.CreatePaymentAsync(payOSRequest);

                        order.PaymentLink = payOSResponse.CheckoutUrl;
                        order.PaymentTransactionId = payOSResponse.OrderCode.ToString();
                        await _orderRepository.UpdateAsync(order);
                    }
                    catch (PayOSException)
                    {
                        // Rollback stock changes
                        foreach (var orderItem in order.OrderItems)
                        {
                            var productVariant = await _productVariantRepository.GetByIdAsync(orderItem.ProductVariantId);
                            if (productVariant != null)
                            {
                                productVariant.StockQuantity += orderItem.Quantity;
                                await _productVariantRepository.UpdateAsync(productVariant);
                            }
                        }
                        await _unitOfWork.CommitAsync();
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
                    throw new AppException(ErrorCode.ORDER_NOT_FOUND);

                if (!IsValidStatusTransition(order, orderStatusRequest.Status))
                    throw new AppException(ErrorCode.INVALID_STATUS_TRANSITION);

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

        public async Task<OrderResponse> RequestReturnAsync(Guid userId, Guid orderId, ReturnRequest returnRequest)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || !order.UserProfile.UserId.Equals(userId))
                throw new AppException(ErrorCode.ORDER_NOT_FOUND);

            if (order.Status != OrderStatus.Delivered)
                throw new AppException(ErrorCode.ORDER_NOT_RETURNABLE);

            if (!IsValidStatusTransition(order, OrderStatus.ReturnRequested))
                throw new AppException(ErrorCode.INVALID_STATUS_TRANSITION);

            order.Status = OrderStatus.ReturnRequested;
            order.ReturnReason = returnRequest.Reason;
            order.UpdatedDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return order.MapModelToResponse();
        }

        public async Task<OrderResponse> UpdateReturnStatusAsync(Guid orderId, ReturnStatusRequest returnStatusRequest)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new AppException(ErrorCode.ORDER_NOT_FOUND);

            if (!IsValidStatusTransition(order, returnStatusRequest.Status))
                throw new AppException(ErrorCode.INVALID_STATUS_TRANSITION);

            order.Status = returnStatusRequest.Status;
            order.UpdatedDate = DateTime.UtcNow;

            if (returnStatusRequest.Status == OrderStatus.Returned)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var productVariant = await _productVariantRepository.GetByIdAsync(orderItem.ProductVariantId);
                    if (productVariant != null)
                    {
                        productVariant.StockQuantity += orderItem.Quantity;
                        await _productVariantRepository.UpdateAsync(productVariant);
                    }
                }
            }

            if (returnStatusRequest.Status == OrderStatus.Refunded)
            {
                order.ReturnReason = returnStatusRequest.RefundReason ?? order.ReturnReason;
            }

            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return order.MapModelToResponse();
        }

        public async Task CancelOrderAsync(Guid userId, Guid orderId, string? cancellationReason, bool callBack = false)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || !order.UserProfile.UserId.Equals(userId))
                    throw new AppException(ErrorCode.ORDER_NOT_FOUND);

                if (!IsValidStatusTransition(order, OrderStatus.Cancelled))
                    throw new AppException(ErrorCode.ORDER_NOT_CANCELLABLE);

                foreach (var orderItem in order.OrderItems)
                {
                    var productVariant = await _productVariantRepository.GetByIdAsync(orderItem.ProductVariantId);
                    if (productVariant == null)
                        throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND);

                    productVariant.StockQuantity += orderItem.Quantity;
                    await _productVariantRepository.UpdateAsync(productVariant);
                }

                if (!callBack && order.PaymentMethod == PaymentMethod.PayOS && !string.IsNullOrEmpty(order.PaymentTransactionId))
                {
                    if (!long.TryParse(order.PaymentTransactionId, out var orderCode))
                        throw new AppException(ErrorCode.PAYMENT_CANCELLATION_FAILED);

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
                order.UpdatedDate = DateTime.UtcNow;
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
                if (paymentInfo == null)
                    return null;

                var order = await _orderRepository.GetByPaymentTransactionIdAsync(paymentInfo.OrderCode.ToString());
                if (order == null)
                    throw new AppException(ErrorCode.ORDER_NOT_FOUND);

                var orderStatusRequest = new OrderStatusRequest
                {
                    Status = paymentInfo.Status switch
                    {
                        "PAID" => OrderStatus.PaymentConfirmed,
                        "CANCELLED" => OrderStatus.Cancelled,
                        _ => OrderStatus.PaymentFailed
                    }
                };

                if (orderStatusRequest.Status == OrderStatus.Cancelled)
                {
                    await CancelOrderAsync(order.UserProfile.UserId, order.Id, "Cancelled via PayOS callback", true);
                    return null;
                }

                return await UpdateOrderStatusAsync(order.Id, orderStatusRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool IsValidStatusTransition(Order order, OrderStatus newStatus)
        {
            var paymentMethod = order.PaymentMethod;
            if (!_validTransitions.TryGetValue(paymentMethod, out var transitions))
                return false;
            return transitions.TryGetValue(order.Status, out var validStatuses) && validStatuses.Contains(newStatus);
        }
    }
}
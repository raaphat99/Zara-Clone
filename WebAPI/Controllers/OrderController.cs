using Amazon.S3.Model;
using Domain.Auth;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Policy;
using WebAPI.DTOs;
using WebAPI.Services;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string TrackingPrefix = "#ZA";
        private readonly ProductService _productService;
        public OrderController(IUnitOfWork unitOfWork, ProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }
        [HttpPut("/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (order.Status == newStatus)
            {
                return BadRequest("Order is already in the specified status.");
            }

            order.Status = newStatus;

            string message = GenerateNotificationMessage(newStatus, order.TrackingNumber);

            var notificationDto = new NotificationDTO
            {
                userId = order.UserId,
                message = message,
                isRead = false,
                created = DateTime.UtcNow
            };

            var notification = new Notification
            {
                UserId = notificationDto.userId,
                Message = notificationDto.message,
                IsRead = notificationDto.isRead,
                Created = notificationDto.created
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.Complete();

            return Ok($"Order status updated to {newStatus} for tracking number {order.TrackingNumber} and notification sent.");
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CheckoutDTO checkout)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new Response { Status = "Error", Message = "Send Valid Data" });

                var userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
                var user = await _unitOfWork.Users.FindSingle(u => u.Id == userId);
                if (user == null)
                    return Unauthorized(new Response { Status = "Error", Message = "Invalid token or email not found in token" });

                // Use a single transaction for all database operations
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    //get tracking number numeric section from database
                    var trackingNumberRecord = await _unitOfWork.TrackingNumbers.GetByIdAsync(1);
                    int trackingNumber = trackingNumberRecord.UniversalTrackingNumber;

                    //create shipping method
                    ShippingMethod method = new ShippingMethod();
                    if (checkout.shippingMethod == "StandardHome" && checkout.totalPrice > 4000)
                    {
                        method.Type = ShippingType.StandardHome;
                        method.ShippingCost = 0;
                    }
                    else if (checkout.shippingMethod == "StandardHome" && checkout.totalPrice < 4000)
                    {
                        method.Type = ShippingType.StandardHome;
                        method.ShippingCost = 89;
                    }
                    if (checkout.shippingMethod == "ZaraStore")
                    {
                        method.Type = ShippingType.ZaraStore;
                        method.ShippingCost = 0;
                    };
                    await _unitOfWork.ShippingMethods.AddAsync(method);
                    await _unitOfWork.Complete();

                    // create the order
                    Order order = new Order
                    {
                        UserId = userId,
                        UserAddressId = checkout.userAddressId,
                        TrackingNumber = $"{TrackingPrefix}{trackingNumber}",
                        TotalPrice = checkout.totalPrice,
                        Created = DateTime.UtcNow,
                        Status = OrderStatus.Pending,
                        ShippingMethodId = method.Id,
                    };
                    
                    // Update tracking number
                    trackingNumberRecord.UniversalTrackingNumber += 1;
                    _unitOfWork.TrackingNumbers.Update(trackingNumberRecord);
                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.Complete();

                    // Create order items
                    var orderItems = checkout.cartItems.Select(item => new OrderItem
                    {
                        ProductVariantId = item.productVariantId,
                        OrderId = order.Id,
                        UnitPrice = item.price ?? 0,
                        Quantity = item.quantity ?? 0,
                        Subtotal = (item.quantity ?? 1 * (item.price ?? 1))
                    }).ToList();

                    // Create payment
                    Payment payment = new Payment
                    {
                        OrderId = order.Id,
                        Amount = order.TotalPrice,
                        Created = DateTime.UtcNow,
                        AmountRefunded = 0,
                        PaymentMethod = checkout.paymentMethod,
                        PaymentStatus = PaymentStatus.Pending,
                    };
                    
                    if (checkout.paymentMethod == "POD")
                    {
                       
                        var cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == order.UserId);
                        if (cart != null)
                        {
                            _unitOfWork.CartItems.RemoveRange(cart.CartItems);
                        }

                        foreach (var item in orderItems)
                        {
                            await _productService.UpdateStockQuantity(item.ProductVariantId ?? 0, item.Quantity);
                        }

                        var notification = new Notification
                        {
                            UserId = order.UserId,
                            Message = $"Order {order.TrackingNumber}  is pending. We are processing it.",
                            Created = DateTime.Now,
                            IsRead = false
                        };

                        await _unitOfWork.Notifications.AddAsync(notification);
                        await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
                        await _unitOfWork.Payments.AddAsync(payment);
                        await _unitOfWork.Complete();

                        await transaction.CommitAsync();
                        return Ok(new Response { Status="Succees",Message="order Confirmed!"});
                    }

                    await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
                    await _unitOfWork.Payments.AddAsync(payment);
                    await _unitOfWork.Complete();

                    await transaction.CommitAsync();

                    string hash = Kashier.create_hash(order.TotalPrice.ToString(), order.Id.ToString());
                    string redirectUrl = $"https://checkout.kashier.io/?merchantId=MID-29117-281&" +
                                         $"orderId={order.Id}&" +
                                         $"amount={order.TotalPrice}&" +
                                         $"currency=EGP&" +
                                         $"hash={hash}&" +
                                         $"mode=test&" +
                                         $"metaData={{\"metaData\":\"myData\"}}&" +
                                         $"merchantRedirect=http://localhost:5250/api/WebHook&" +
                                         $"allowedMethods=card,bank_installments,wallet&" +
                                         $"failureRedirect=true&" +
                                         $"redirectMethod=post&" +
                                         $"brandColor=%23000000&" +
                                         $"display=en";
                    return Ok(new OrderResponse { Status = "Success", RedirectUrl = redirectUrl });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new Response { Status = "Error", Message = "An error occurred while processing your order" });
            }
        }
        private string GenerateNotificationMessage(OrderStatus newStatus, string trackingNumber)
        {
            return newStatus switch
            {
                OrderStatus.Pending => $"Order {trackingNumber} is pending. We are processing it.",
                OrderStatus.Shipped => $"Good news! Order {trackingNumber} has been shipped.",
                OrderStatus.Delivered => $"Your order {trackingNumber} has been delivered successfully.",
                OrderStatus.Canceled => $"We regret to inform you that order {trackingNumber} has been canceled.",
                _ => "Unknown order status."
            };
        }

    }

}



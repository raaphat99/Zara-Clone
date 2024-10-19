using DataAccess.EFCore.Migrations;
using Domain.Auth;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;
using WebAPI.Services;
namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductService _productService;
        public WebHookController(IUnitOfWork unitOfWork, ProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        [HttpPost]
        public async Task<ActionResult> ReceiveWebhook([FromForm] string paymentStatus, [FromForm] string merchantOrderId)
        {
            // var status = HttpContext.Request.Query["paymentStatus"];
            if (paymentStatus == "SUCCESS")
            {
                int OrderId = int.Parse(merchantOrderId);
                var order = await _unitOfWork.Orders.FindSingle(o => o.Id == OrderId);
                order.Status = OrderStatus.Shipped; //update order status

                var payment = await _unitOfWork.Payments.FindSingle(o => o.Id == order.Payment.Id);
                payment.PaymentStatus = PaymentStatus.Paid; //update payment status

                var cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == order.UserId);
                cart.CartItems.Clear();            //clear cart for this user

                foreach (var item in order.OrderItems)
                {
                    await _productService.UpdateStockQuantity(item.ProductVariantId ?? 0, item.Quantity); //update stock quantity
                }

                Notification notification = new Notification   // notify user
                {
                    UserId = order.UserId,
                    Message = $"Good news!,order payment was successful and the Order {order.TrackingNumber} has been shipped.",
                    Created = DateTime.Now,
                    IsRead = false
                };


                await _unitOfWork.Notifications.AddAsync(notification);
                _unitOfWork.Orders.Update(order);
                _unitOfWork.Payments.Update(payment);
                _unitOfWork.Carts.Update(cart);
                await _unitOfWork.Complete();
                return Redirect("http://localhost:4200/home");
            }
            else
            {
                int OrderId = int.Parse(merchantOrderId);
                var order = await _unitOfWork.Orders.FindSingle(o => o.Id == OrderId);
                var payment = await _unitOfWork.Payments.FindSingle(o => o.Id == order.Payment.Id);
                payment.PaymentStatus = PaymentStatus.Canceled; //update payment status
                Notification notification = new Notification   // notify user
                {
                    UserId = order.UserId,
                    Message = $"Payment was not Accepted, please enter your card details correctly or contact the Bank",
                    Created = DateTime.Now,
                    IsRead = false
                };
                return Ok(new OrderResponse { Status="Failed"});
            }


            
        }
    }
}

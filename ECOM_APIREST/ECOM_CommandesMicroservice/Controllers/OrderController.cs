using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECOM_CommandesMicroservice.Models;
using ECOM_CommandesMicroservice.Services;
using System.Security.Claims;

namespace ECOM_CommandesMicroservice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var authorizationToken = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationToken))
                    return Unauthorized();

                var order = await _orderService.CreateOrderFromCartAsync(
                    userId,
                    request.ShippingAddress,
                    request.ShippingCity,
                    request.ShippingPostalCode,
                    request.ShippingCountry,
                    authorizationToken
                );

                return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrder(int orderId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var order = await _orderService.GetOrderAsync(orderId, userId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<Order>>> GetUserOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{orderId}/status")]
        public async Task<ActionResult<Order>> UpdateOrderStatus(int orderId, [FromBody] OrderStatus newStatus)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var order = await _orderService.UpdateOrderStatusAsync(orderId, userId, newStatus);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                await _orderService.DeleteOrderAsync(orderId, userId);
                return Ok("commande supprimée avec succès");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateOrderRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingPostalCode { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
    }
}

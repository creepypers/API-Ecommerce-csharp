using Microsoft.EntityFrameworkCore;
using ECOM_CommandesMicroservice.Models;
using ECOM_CommandesMicroservice.Data;
using System.Text.Json;

namespace ECOM_CommandesMicroservice.Services
{
    public class OrderService
    {
        private readonly OrderDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OrderService(OrderDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId, string shippingAddress, string shippingCity, string shippingPostalCode, string shippingCountry, string authorizationToken)
        {
            var cartServiceUrl = _configuration["ServiceUrls:CartService"].ToString();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationToken);

            var cartResponse = await _httpClient.GetAsync($"{cartServiceUrl}/api/cart");
            if (!cartResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve cart information: {cartResponse.StatusCode}");
            }

            var cartJson = await cartResponse.Content.ReadAsStringAsync();
            var cartData = JsonDocument.Parse(cartJson).RootElement;

            var order = new Order
            {
                UserId = userId,
                Subtotal = cartData.GetProperty("subtotal").GetDecimal(),
                ShippingCost = cartData.GetProperty("shippingCost").GetDecimal(),
                Tax = cartData.GetProperty("tax").GetDecimal(),
                TotalPrice = cartData.GetProperty("total").GetDecimal(),
                TotalItems = cartData.GetProperty("totalItems").GetInt32(),
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress,
                ShippingCity = shippingCity,
                ShippingPostalCode = shippingPostalCode,
                ShippingCountry = shippingCountry,
                Items = new List<OrderItem>()
            };

            var items = cartData.GetProperty("items").EnumerateArray();
            foreach (var item in items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.GetProperty("productId").GetInt32(),
                    Name = item.GetProperty("name").GetString() ?? "",
                    ImageUrl = item.GetProperty("imageUrl").GetString(),
                    Price = item.GetProperty("price").GetDecimal(),
                    Quantity = item.GetProperty("quantity").GetInt32(),
                    Description = item.GetProperty("description").GetString(),
                    Category = item.GetProperty("category").GetString()
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _httpClient.DeleteAsync($"{cartServiceUrl}/api/cart/clear");

            return order;
        }

        public async Task<Order> GetOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found");
            }

            return order;
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, string userId, OrderStatus newStatus)
        {
            var order = await GetOrderAsync(orderId, userId);

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
            {
                throw new Exception($"Cannot modify a {order.Status.ToString().ToLower()} order");
            }

            if (order.Status == OrderStatus.Processing || order.Status == OrderStatus.Pending)
            {
                if (newStatus != OrderStatus.Cancelled && newStatus != OrderStatus.Processing)
                {
                    throw new Exception($"You can only cancel or process a {order.Status.ToString().ToLower()} order");
                }
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Shipped)
                order.ShippedAt = DateTime.UtcNow;
            else if (newStatus == OrderStatus.Delivered)
                order.DeliveredAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrderAsync(int orderId, string userId)
        {
            var order = await GetOrderAsync(orderId, userId);

            if (order.Status != OrderStatus.Pending)
            {
                throw new Exception("Only pending orders can be deleted");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
} 
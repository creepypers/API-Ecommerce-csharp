using ECOM_PanierMicroservice.Models;
using ECOM_PanierMicroservice.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;

namespace ECOM_PanierMicroservice.Services
{
    public class CartService
    {
        private readonly CartDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public CartService(CartDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        private CartResponse MapToResponse(Cart cart)
        {
            return new CartResponse
            {
                UserId = cart.UserId,
                Status = cart.Status,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.Items.Select(item => new CartItemResponse
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    ImageUrl = item.ImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Description = item.Description,
                    Category = item.Category,
                    AddedAt = item.AddedAt,
                    UpdatedAt = item.UpdatedAt
                }).ToList()
            };
        }

        public async Task<CartResponse> GetCartAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return MapToResponse(cart);
        }

        public async Task<CartResponse> AddToCartAsync(string userId, AddToCartRequest request)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty");
            
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ProductsService");
                var product = await httpClient.GetFromJsonAsync<ProductInfo>($"api/products/{request.ProductId}");
                
                if (product == null)
                    throw new ArgumentException($"Product with ID {request.ProductId} not found");

                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart { UserId = userId };
                
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
                
                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        ProductId = request.ProductId,
                        CartId = cart.UserId,
                        Name = product.Name,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = request.Quantity,
                        Description = product.Description,
                        Category = product.Category
                    };
                    cart.Items.Add(newItem);
                }
                
                if (cart.UserId == null)
                {
                    _context.Carts.Add(cart);
                }
                
                await _context.SaveChangesAsync();
                return MapToResponse(cart);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error communicating with Products service: {ex.Message}", ex);
            }
        }

        public async Task<CartResponse> UpdateQuantityAsync(string userId, int productId, int quantity)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty");
            
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null)
                throw new ArgumentException($"Cart not found for user {userId}");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                    item.UpdatedAt = DateTime.UtcNow;
                }
                
                await _context.SaveChangesAsync();
            }
            
            return MapToResponse(cart);
        }

        public async Task<CartResponse> RemoveFromCartAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty");
            
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null)
                throw new ArgumentException($"Cart not found for user {userId}");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null)
            {
                cart.Items.Remove(item);
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            
            return MapToResponse(cart);
        }

        public async Task<CartResponse> ClearCartAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty");
            
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null)
                throw new ArgumentException($"Cart not found for user {userId}");

            _context.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear();
            await _context.SaveChangesAsync();
            
            return MapToResponse(cart);
        }
    }
} 
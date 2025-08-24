using System.Text.Json.Serialization;

namespace ECOM_PanierMicroservice.Models
{
    public class CartResponse
    {
        public string UserId { get; set; } = string.Empty;
        public CartStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal ShippingCost => Subtotal > 0 ? 10.00m : 0;
        public decimal Tax => Subtotal * 0.07m;
        public decimal Total => Subtotal + ShippingCost + Tax;
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    public class CartItemResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Subtotal => Price * Quantity;
        public DateTime AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 
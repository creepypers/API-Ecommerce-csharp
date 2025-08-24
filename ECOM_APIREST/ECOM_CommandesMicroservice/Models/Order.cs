using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECOM_CommandesMicroservice.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int OrderId { get; set; }
        
        [Required(ErrorMessage = "L'identifiant de l'utilisateur est obligatoire")]
        [Display(Name = "User ID")]
        public string UserId { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        
        [Column(TypeName = "nvarchar(20)")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        [Required(ErrorMessage = "L'adresse de livraison est obligatoire")]
        [StringLength(100)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La ville de livraison est obligatoire")]
        [StringLength(50)]
        public string ShippingCity { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le code postal est obligatoire")]
        [StringLength(20)]
        public string ShippingPostalCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le pays de livraison est obligatoire")]
        [StringLength(50)]
        public string ShippingCountry { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public int TotalItems { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual Order? Order { get; set; }
        public int OrderId { get; set; }
    }
}

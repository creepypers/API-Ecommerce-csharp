using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECOM_PanierMicroservice.Models
{
    public class CartItem
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public string CartId { get; set; } = string.Empty;
        
        [ForeignKey("CartId")]
        [JsonIgnore]
        public virtual Cart? Cart { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Url]
        public string? ImageUrl { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        public decimal Subtotal => Price * Quantity;
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 
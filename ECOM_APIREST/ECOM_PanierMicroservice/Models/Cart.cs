using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECOM_PanierMicroservice.Models
{
    public class Cart
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        
        [JsonIgnore]
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        
        [Required]
        public CartStatus Status { get; set; } = CartStatus.Active;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    
    public enum CartStatus
    {
        Active,
        CheckedOut,
        Abandoned
    }
} 
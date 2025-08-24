using System.ComponentModel.DataAnnotations;

namespace ECOM_PanierMicroservice.Models
{
    public class AddToCartRequest
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }
} 
using System.Text.Json.Serialization;

namespace ECOM_PayementMicroservice.Models
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }
    }
} 
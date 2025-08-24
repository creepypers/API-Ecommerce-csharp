using System;

namespace ECOM_PayementMicroservice.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string OrderReference { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Failed,
        Cancelled
    }
} 
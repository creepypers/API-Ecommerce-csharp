using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Newtonsoft.Json;
using ECOM_PayementMicroservice.Models;

namespace ECOM_PayementMicroservice.Services
{
    public class StripeService
    {
        private readonly string _stripeSecretKey;
        private const decimal MAX_AMOUNT = 999999.99m;
        private const string DEFAULT_CURRENCY = "cad";
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _stripeSecretKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _stripeSecretKey;
            _configuration = configuration;
        }

        public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                return await service.GetAsync(paymentIntentId);
            }
            catch (StripeException ex)
            {
                throw;
            }
        }

        public async Task<PaymentIntent> UpdatePaymentIntentAsync(string paymentIntentId, decimal amount)
        {
            try
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)(amount * 100)
                };

                var service = new PaymentIntentService();
                return await service.UpdateAsync(paymentIntentId, options);
            }
            catch (StripeException ex)
            {
                throw;
            }
        }

        public async Task<PaymentIntent> CancelPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                return await service.CancelAsync(paymentIntentId);
            }
            catch (StripeException ex)
            {
                throw;
            }
        }

        public async Task<PaymentTransaction> CreatePaymentIntentForOrderAsync(int orderId, decimal amount)
        {
            try
            {
                if (amount < 0.50m)
                {
                    throw new ArgumentException("Le montant doit être d'au moins 0,50 CAD");
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "cad",
                    PaymentMethodTypes = new List<string> { "card" },
                    PaymentMethod = "pm_card_visa",
                    Confirm = true,
                    Metadata = new Dictionary<string, string>
                    {
                        { "orderId", orderId.ToString() }
                    }
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                return new PaymentTransaction
                {
                    Id = orderId,
                    OrderId = orderId,
                    PaymentIntentId = intent.Id,
                    Amount = amount,
                    Currency = "cad",
                    Status = intent.Status,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                throw;
            }
        }

        public async Task<PaymentTransaction> ConfirmPaymentForOrderAsync(string paymentIntentId, string paymentMethodId, CreditCardInfo creditCard)
        {
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethodId,
                    ReturnUrl = _configuration["Stripe:ReturnUrl"]
                };

                var paymentIntent = await service.ConfirmAsync(paymentIntentId, options);

                return new PaymentTransaction
                {
                    PaymentIntentId = paymentIntent.Id,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount / 100m,
                    Currency = paymentIntent.Currency,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                throw;
            }
        }

        public async Task<PaymentTransaction> GetPaymentStatusAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var intent = await service.GetAsync(paymentIntentId);

                return new PaymentTransaction
                {
                    PaymentIntentId = intent.Id,
                    Amount = intent.Amount / 100m,
                    Currency = intent.Currency,
                    Status = intent.Status
                };
            }
            catch (StripeException ex)
            {
                throw;
            }
        }

        private void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Le montant doit être supérieur à zéro");
            }
            if (amount > MAX_AMOUNT)
            {
                throw new ArgumentException($"Le montant ne peut pas dépasser {MAX_AMOUNT}");
            }
        }
    }
} 
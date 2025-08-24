using ECOM_PayementMicroservice.Services;
using ECOM_PayementMicroservice.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECOM_PayementMicroservice.Controllers
{
    [Route("api/payment")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly StripeService _stripeService;
        private readonly OrderService _orderService;

        public PaymentController(StripeService stripeService, OrderService orderService)
        {
            _stripeService = stripeService;
            _orderService = orderService;
        }

        [HttpPost("create-order-payment")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOrderPayment([FromBody] CreateOrderPaymentRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var authorizationToken = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationToken))
                {
                    return Unauthorized(new { error = "Token d'autorisation manquant" });
                }

                var orderDetails = await _orderService.GetOrderDetailsAsync(request.OrderId, authorizationToken);
                if (orderDetails == null)
                {
                    return NotFound($"Commande {request.OrderId} non trouvée");
                }

                var transaction = await _stripeService.CreatePaymentIntentForOrderAsync(
                    request.OrderId,
                    orderDetails.TotalPrice
                );

                transaction.UserId = int.Parse(userId);
                transaction.OrderId = request.OrderId;

                return CreatedAtAction(nameof(GetPaymentStatus), new { paymentIntentId = transaction.PaymentIntentId }, transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (StripeException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur est survenue lors de la création du paiement" });
            }
        }

        [HttpPost("{paymentIntentId}/confirm")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConfirmOrderPayment(string paymentIntentId, [FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(request.PaymentMethodId))
                {
                    return BadRequest(new { error = "L'identifiant de la méthode de paiement est requis" });
                }

                if (request.CreditCard.ExpYear < DateTime.Now.Year || 
                    (request.CreditCard.ExpYear == DateTime.Now.Year && request.CreditCard.ExpMonth < DateTime.Now.Month))
                {
                    return BadRequest(new { error = "La carte de crédit a expiré" });
                }

                var transaction = await _stripeService.ConfirmPaymentForOrderAsync(
                    paymentIntentId, 
                    request.PaymentMethodId,
                    request.CreditCard
                );

                return Ok(transaction);
            }
            catch (StripeException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.Message + " " + ex.StripeError.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur est survenue lors de la confirmation du paiement" });
            }
        }

        [HttpGet("{paymentIntentId}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPaymentStatus(string paymentIntentId)
        {
            try
            {
                var transaction = await _stripeService.GetPaymentStatusAsync(paymentIntentId);
                return Ok(transaction);
            }
            catch (StripeException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur est survenue lors de la récupération du statut du paiement" });
            }
        }

        [HttpGet("{paymentIntentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPaymentIntent(string paymentIntentId)
        {
            try
            {
                var intent = await _stripeService.GetPaymentIntentAsync(paymentIntentId);
                return Ok(intent);
            }
            catch (StripeException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur est survenue lors de la récupération du paiement" });
            }
        }

        [HttpPut("{paymentIntentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdatePaymentIntent(string paymentIntentId, [FromBody] UpdatePaymentRequest request)
        {
            try
            {
                var intent = await _stripeService.UpdatePaymentIntentAsync(paymentIntentId, request.Amount);
                return Ok(intent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (StripeException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur est survenue lors de la mise à jour du paiement" });
            }
        }

        [HttpPost("{paymentIntentId}/cancel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CancelPaymentIntent(string paymentIntentId)
        {
            try
            {
                var intent = await _stripeService.CancelPaymentIntentAsync(paymentIntentId);
                return Ok(intent);
            }
            catch (StripeException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur est survenue lors de l'annulation du paiement" });
            }
        }
    }

    public class CreateOrderPaymentRequest
    {
        [Required(ErrorMessage = "L'ID de la commande est requis")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de la commande doit être un nombre positif")]
        public int OrderId { get; set; }
    }

    public class ConfirmPaymentRequest
    {
        [Required(ErrorMessage = "L'identifiant de la méthode de paiement est requis")]
        public string PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Les informations de la carte de crédit sont requises")]
        public CreditCardInfo CreditCard { get; set; }
    }

    public class UpdatePaymentRequest
    {
        [Required]
        public decimal Amount { get; set; }
    }
} 
using System.ComponentModel.DataAnnotations;

namespace ECOM_PayementMicroservice.Models
{
    public class CreditCardInfo
    {
        [Required(ErrorMessage = "Le numéro de carte est requis")]
        [CreditCard(ErrorMessage = "Le numéro de carte n'est pas valide")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Le mois d'expiration est requis")]
        [Range(1, 12, ErrorMessage = "Le mois d'expiration doit être entre 1 et 12")]
        public int ExpMonth { get; set; }

        [Required(ErrorMessage = "L'année d'expiration est requise")]
        [Range(2024, 2030, ErrorMessage = "L'année d'expiration doit être valide")]
        public int ExpYear { get; set; }

        [Required(ErrorMessage = "Le code CVC est requis")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Le code CVC doit contenir 3 ou 4 chiffres")]
        public string Cvc { get; set; }
    }
} 
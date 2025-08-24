using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECOM_AuthentificationMicroservice.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
} 
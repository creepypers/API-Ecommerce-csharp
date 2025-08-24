using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECOM_AuthentificationMicroservice.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le type d'utilisateur est obligatoire")]
        [JsonPropertyName("userType")]
        public UserType UserType { get; set; } = UserType.Client;
        
        [JsonPropertyName("companyName")]
        public string? CompanyName { get; set; }
        
        [JsonPropertyName("companyAddress")]
        public string? CompanyAddress { get; set; }
        
        [JsonPropertyName("companyPhone")]
        public string? CompanyPhone { get; set; }
        
        [JsonPropertyName("companyDescription")]
        public string? CompanyDescription { get; set; }
    }
} 
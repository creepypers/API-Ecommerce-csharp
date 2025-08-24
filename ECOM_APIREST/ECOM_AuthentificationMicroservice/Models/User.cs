using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOM_AuthentificationMicroservice.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [JsonPropertyName("password")]
        public string? Password { get; set; }
        
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("userType")]
        public UserType UserType { get; set; } = UserType.Client;
        
        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [JsonPropertyName("companyName")]
        public string? CompanyName { get; set; }
        
        [JsonPropertyName("companyAddress")]
        public string? CompanyAddress { get; set; }
        
        [JsonPropertyName("companyPhone")]
        public string? CompanyPhone { get; set; }
        
        [JsonPropertyName("companyDescription")]
        public string? CompanyDescription { get; set; }
    }
    
    public enum UserType
    {
        Client = 0,
        Vendeur = 1
    }
} 
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOM_UtilisateurMicroservice.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string? Password { get; set; }
        
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string? FirstName { get; set; }
        
        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Le type de compte est obligatoire")]
        public UserType UserType { get; set; }
        
        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyDescription { get; set; }
    }
    
    public enum UserType
    {
        Client,
        Vendeur
    }
} 
using ECOM_AuthentificationMicroservice.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECOM_AuthentificationMicroservice.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? 
                "ECOM_tp_suPer_Secret_Key_For_JWT_Auth_2024_Default";
            var issuer = _configuration["Jwt:Issuer"] ?? "ECOM_AuthentificationMicroservice";
            var audience = _configuration["Jwt:Audience"] ?? "ECOM_API_Clients";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            if (user.UserType == UserType.Vendeur)
            {
                claims.Add(new Claim("isVendeur", "true"));

                if (!string.IsNullOrEmpty(user.CompanyName))
                    claims.Add(new Claim("companyName", user.CompanyName));
                
                if (!string.IsNullOrEmpty(user.CompanyAddress))
                    claims.Add(new Claim("companyAddress", user.CompanyAddress));
                
                if (!string.IsNullOrEmpty(user.CompanyPhone))
                    claims.Add(new Claim("companyPhone", user.CompanyPhone));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length < 10)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            
            if (!tokenHandler.CanReadToken(token))
                return false;
            
            try
            {
                var jwt = tokenHandler.ReadJwtToken(token);
                var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp");
                if (expClaim != null)
                {
                    var expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                    if (expiration <= DateTimeOffset.UtcNow)
                        return false;
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public User GetUserFromToken(string token)
        {
            if (!ValidateToken(token))
                return null;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            
            var user = new User
            {
                Id = int.Parse(claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                FirstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                LastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value
            };
            
            var roleValue = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            user.UserType = roleValue == "Vendeur" ? UserType.Vendeur : UserType.Client;
            
            if (user.UserType == UserType.Vendeur)
            {
                user.CompanyName = claims.FirstOrDefault(c => c.Type == "companyName")?.Value;
                user.CompanyAddress = claims.FirstOrDefault(c => c.Type == "companyAddress")?.Value;
                user.CompanyPhone = claims.FirstOrDefault(c => c.Type == "companyPhone")?.Value;
            }
            
            return user;
        }
        
        private SymmetricSecurityKey GetSecurityKey()
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? 
                "ECOM_tp_suPer_Secret_Key_For_JWT_Auth_2024_Default";
                
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
    }
} 
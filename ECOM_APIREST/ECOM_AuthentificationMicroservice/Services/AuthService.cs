using ECOM_AuthentificationMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace ECOM_AuthentificationMicroservice.Services
{
    public class AuthService
    {
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        
        private readonly string _userServiceUrl;
        private readonly string _productServiceUrl;
        private readonly string _cartServiceUrl;
        private readonly string _orderServiceUrl;
        private readonly string _paymentServiceUrl;

        public AuthService(TokenService tokenService, IConfiguration configuration, HttpClient httpClient)
        {
            _tokenService = tokenService;
            _configuration = configuration;
            _httpClient = httpClient;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            _userServiceUrl = _configuration["ExternalServices:UserService"] ?? "http://localhost:8001";
            _productServiceUrl = _configuration["ExternalServices:ProductService"] ?? "http://localhost:8002";
            _cartServiceUrl = _configuration["ExternalServices:CartService"] ?? "http://localhost:8003";
            _orderServiceUrl = _configuration["ExternalServices:OrderService"] ?? "http://localhost:8004";
            _paymentServiceUrl = _configuration["ExternalServices:PaymentService"] ?? "http://localhost:8005";
        }

        public bool ValidateToken(string token)
        {
            return _tokenService.ValidateToken(token);
        }
        
        public User GetUserFromToken(string token)
        {
            return _tokenService.GetUserFromToken(token);
        }

        public async Task<(bool success, string token, object user)> AuthenticateAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return (false, string.Empty, null);
            }

            if (user.Password != password)
            {
                return (false, string.Empty, null);
            }

            var token = _tokenService.CreateToken(user);
            
            var response = CreateUserResponse(user);
            
            return (true, token, response);
        }

        public async Task<(bool success, string message)> RegisterAsync(User newUser)
        {
            var existingUser = await GetUserByEmailAsync(newUser.Email);
            if (existingUser != null)
            {
                return (false, "User with this email already exists");
            }

            try
            {
                var requestObj = new 
                {
                    email = newUser.Email,
                    password = newUser.Password,
                    firstName = newUser.FirstName,
                    lastName = newUser.LastName,
                    userType = (int)newUser.UserType,
                    companyName = newUser.CompanyName,
                    companyAddress = newUser.CompanyAddress,
                    companyPhone = newUser.CompanyPhone,
                    companyDescription = newUser.CompanyDescription
                };
                
                var json = JsonSerializer.Serialize(requestObj, _jsonOptions);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_userServiceUrl}/api/users", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, "User registered successfully");
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to register user: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
        
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"{_userServiceUrl}/api/users/{userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            var userContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(userContent, _jsonOptions);
        }
        
        private async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_userServiceUrl}/api/users/by-email/{email}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                
                var userContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<User>(userContent, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }
        
        private object CreateUserResponse(User user)
        {
            if (user.UserType == UserType.Vendeur)
            {
                return new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.UserType.ToString(),
                    companyName = user.CompanyName,
                    companyAddress = user.CompanyAddress,
                    companyPhone = user.CompanyPhone,
                    companyDescription = user.CompanyDescription
                };
            }
            
            return new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                role = user.UserType.ToString()
            };
        }
    }
}
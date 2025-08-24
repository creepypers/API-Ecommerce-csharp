using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ECOM_UtilisateurMicroservice.Models;

namespace ECOM_UtilisateurMicroservice.Services
{
    public class DummyJsonService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://dummyjson.com";

        public DummyJsonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/users?limit=100");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var dummyUsers = JsonSerializer.Deserialize<DummyUsersResponse>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var users = new List<User>();
            
            foreach (var dummyUser in dummyUsers.Users)
            {
                var userType = dummyUser.Id <= 50 
                    ? UserType.Vendeur 
                    : UserType.Client;
                
                var user = new User
                {
                    Email = dummyUser.Email,
                    Password = dummyUser.Password,
                    FirstName = dummyUser.FirstName,
                    LastName = dummyUser.LastName,
                    UserType = userType,
                    CompanyName = userType == UserType.Vendeur ? dummyUser.Company.Name : null,
                    CompanyAddress = userType == UserType.Vendeur ? 
                        $"{dummyUser.Company.Address.Address}, {dummyUser.Company.Address.City}, {dummyUser.Company.Address.State}" : null,
                    CompanyPhone = userType == UserType.Vendeur ? dummyUser.Phone : null,
                    CompanyDescription = userType == UserType.Vendeur ? 
                        $"{dummyUser.Company.Title} at {dummyUser.Company.Name}" : null,
                    CreatedAt = DateTime.Now.AddDays(-Random.Shared.Next(1, 365))
                };
                
                users.Add(user);
            }
            
            return users;
        }
    }

    public class DummyUsersResponse
    {
        public List<DummyUser>? Users { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }

    public class DummyUser
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DummyCompany? Company { get; set; }
    }

    public class DummyCompany
    {
        public string? Department { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public DummyAddress? Address { get; set; }
    }

    public class DummyAddress
    {
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
} 
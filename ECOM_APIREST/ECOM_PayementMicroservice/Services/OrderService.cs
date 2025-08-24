using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ECOM_PayementMicroservice.Models;

namespace ECOM_PayementMicroservice.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<OrderResponse> GetOrderDetailsAsync(int orderId, string authorizationToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationToken);

                var response = await _httpClient.GetAsync($"{_configuration["OrderService:BaseUrl"]}/api/Order/{orderId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<OrderResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return order;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Impossible de récupérer les détails de la commande: {ex.Message}");
            }
        }
    }
} 
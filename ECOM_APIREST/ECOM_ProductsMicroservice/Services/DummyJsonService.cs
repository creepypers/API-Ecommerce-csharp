using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ECOM_ProductsMicroservice.Models;

namespace ECOM_ProductsMicroservice.Services
{
    public class DummyJsonService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://dummyjson.com";

        public DummyJsonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/products?limit=100");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var dummyProducts = JsonSerializer.Deserialize<DummyProductsResponse>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var products = new List<Product>();
            var random = new Random();
            
            foreach (var dummyProduct in dummyProducts.Products)
            {
                var sellerId = random.Next(50);
                
                var product = new Product
                {
                    Name = dummyProduct.Title,
                    ShortDescription = dummyProduct.Description.Length > 100 
                        ? dummyProduct.Description.Substring(0, 100) + "..." 
                        : dummyProduct.Description,
                    Description = dummyProduct.Description,
                    Price = dummyProduct.Price,
                    Category = dummyProduct.Category,
                    ImageUrl = dummyProduct.Thumbnail,
                    IsNewArrival = dummyProduct.Stock < 10,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 90)),
                    SellerId = sellerId,
                    Rating = dummyProduct.Rating
                };
                
                products.Add(product);
            }
            
            return products;
        }
    }

    public class DummyProductsResponse
    {
        public List<DummyProduct> Products { get; set; }
    }

    public class DummyProduct
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Thumbnail { get; set; }
    }
} 
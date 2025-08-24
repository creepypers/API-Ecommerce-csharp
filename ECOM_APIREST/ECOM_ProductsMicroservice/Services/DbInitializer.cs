using ECOM_ProductsMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOM_ProductsMicroservice.Services
{
    public class DbInitializer
    {
        private readonly ProductDbContext _context;
        private readonly DummyJsonService _dummyJsonService;

        public DbInitializer(ProductDbContext context, DummyJsonService dummyJsonService)
        {
            _context = context;
            _dummyJsonService = dummyJsonService;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (_context.Products.Any())
            {
                return;
            }
            
            var products = await _dummyJsonService.GetProductsAsync();
            
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
} 
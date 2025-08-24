using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECOM_ProductsMicroservice
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Models.Product> Products { get; set; }
        
        private readonly IConfiguration _configuration;
        
        public ProductDbContext(DbContextOptions<ProductDbContext> options, IConfiguration configuration = null) 
            : base(options)
        {
            _configuration = configuration;
        }
        
        public ProductDbContext()
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ECOM_CommandesMicroservice.Models;

namespace ECOM_CommandesMicroservice.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ShippingCost).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Tax).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Status)
                    .HasConversion(
                        v => v.ToString(),
                        v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v))
                    .HasMaxLength(20);
                entity.Property(e => e.ShippingAddress).HasMaxLength(100);
                entity.Property(e => e.ShippingCity).HasMaxLength(50);
                entity.Property(e => e.ShippingPostalCode).HasMaxLength(20);
                entity.Property(e => e.ShippingCountry).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(o => o.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.AddedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}

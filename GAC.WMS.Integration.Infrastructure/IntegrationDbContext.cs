using GAC.WMS.Integration.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Integration.Infrastructure
{
    public class IntegrationDbContext : DbContext
    {
        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options)
        {
        }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.CustomerId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(p => p.PurchaseOrder)
                .WithMany(p => p.Items)
                .HasForeignKey(p => p.PurchaseOrderId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductCode);

            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<SalesOrderItem>()
                .HasOne(s => s.SalesOrder)
                .WithMany(s => s.Items)
                .HasForeignKey(s => s.SalesOrderId);

            modelBuilder.Entity<SalesOrderItem>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductCode);
        }
    }
}

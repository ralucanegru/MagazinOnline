using Microsoft.EntityFrameworkCore;
using MagazinOnline.Data.Models;
using MagazinOnline.Data.Models.Orders;
using MagazinOnline.Data.Models.Products;

namespace MagazinOnline.Data.Context
{
    /// <summary>
    /// EntityFramework DbContext used by this application to manipulate all data in DB.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructs a new instance using <see cref="Microsoft.EntityFrameworkCore.DbContextOptions"/>
        /// </summary>
        /// <param name="options"><see cref="Microsoft.EntityFrameworkCore.DbContextOptions"/></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrdersProduct> OrderProducts { get; set; }

        /// <summary>
        /// Called when EF Core builds its model.
        /// </summary>
        /// <param name="modelBuilder">The EF Core model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrdersProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.OrderId });

            });
        }
    }
}

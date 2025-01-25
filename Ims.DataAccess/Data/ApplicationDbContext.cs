using Ims.Models;
using Microsoft.EntityFrameworkCore;

namespace Ims.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Snacks", Description = "Time to munch" },
                new Category { CategoryId = 2, CategoryName = "Stationary", Description = "Time to study" },
                new Category { CategoryId = 3, CategoryName = "Electronics", Description = "Gotta get that" }
                );

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

                entity.HasData(
                    new Product
                    {
                        ProductId = 1,
                        ProductName = "Sample Product 1",
                        UnitPrice = 10.99m,
                        QuantityStock = 100,
                        ReorderLevel = 10,
                        CategoryId = 1,
                        ImageUrl = ""
                    },
                    new Product
                    {
                        ProductId = 2,
                        ProductName = "Sample Product 2",
                        UnitPrice = 20.99m,
                        QuantityStock = 200,
                        ReorderLevel = 20,
                        CategoryId = 2,
                        ImageUrl = ""
                    },
                    new Product
                    {
                        ProductId = 5,
                        ProductName = "Sample Product 3",
                        UnitPrice = 30.99m,
                        QuantityStock = 300,
                        ReorderLevel = 30,
                        CategoryId = 3,
                        ImageUrl = ""
                    }
                );
            });
        }
    }
}

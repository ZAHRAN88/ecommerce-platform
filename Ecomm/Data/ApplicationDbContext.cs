using Ecomm.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category 
                { 
                    Id = 1, 
                    Name = "Footwear", 
                    Description = "Shoes, sneakers, and other footwear" 
                },
                new Category 
                { 
                    Id = 2, 
                    Name = "Electronics", 
                    Description = "Electronic devices and accessories" 
                },
                new Category 
                { 
                    Id = 3, 
                    Name = "Accessories", 
                    Description = "Fashion accessories and bags" 
                },
                new Category 
                { 
                    Id = 4, 
                    Name = "Home & Kitchen", 
                    Description = "Home appliances and kitchen items" 
                }
            );

            // Seed products with proper category IDs
            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, 
                    Name = "Nike Air Max", 
                    Description = "Classic Nike Air Max sneakers with air cushioning technology for maximum comfort.", 
                    Price = 129.99M, 
                    ImageUrl = "/images/products/nike-air-max.png",
                    CategoryId = 1 // Footwear
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Wireless Headphones", 
                    Description = "High-quality wireless headphones with noise cancellation and 20-hour battery life.", 
                    Price = 89.99M, 
                    ImageUrl = "/images/products/wireless-headphones.png",
                    CategoryId = 2 // Electronics
                },
                new Product 
                { 
                    Id = 3, 
                    Name = "Smart Watch", 
                    Description = "Feature-rich smartwatch with heart rate monitoring, GPS, and water resistance.", 
                    Price = 199.99M, 
                    ImageUrl = "./images/products/smart-watch.png",
                    CategoryId = 2 // Electronics
                },
                new Product 
                { 
                    Id = 4, 
                    Name = "Laptop Backpack", 
                    Description = "Durable laptop backpack with multiple compartments and USB charging port.", 
                    Price = 49.99M, 
                    ImageUrl = "/images/products/laptop-backpack.png",
                    CategoryId = 3 // Accessories
                },
                new Product 
                { 
                    Id = 5, 
                    Name = "Coffee Maker", 
                    Description = "Programmable coffee maker with 12-cup capacity and built-in grinder.", 
                    Price = 79.99M, 
                    ImageUrl = "/images/products/coffee-maker.png",
                    CategoryId = 4 // Home & Kitchen
                }
            );
        }
    }
}
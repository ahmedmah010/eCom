using eCom.Models;
using eCom.Models.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Loads all EntityTypeConfiguration from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryTC).Assembly);
           
            //Seeds
            /*
            modelBuilder.Entity<Category>().HasData(
                new Category {Id=1,Name = "PC" },
                new Category { Id = 2, Name = "Mobile" }
                );
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name ="Men"}
                );
            modelBuilder.Entity<Product>().HasData(
                new Product {Id=1,Brand = "Samsung", CurrentPrice = 100, CurrentQuantity = 4, Description = "test",
                    Image = "none", Title = "phone", CategoryId = 1 });
            modelBuilder.Entity<ProductTag>().HasData(
                new ProductTag { ProductId=1,TagId=1}
                );
            */



            base.OnModelCreating(modelBuilder);
        }
    }
}

using eCom.Models;
using eCom.Models.EntityTypeConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        

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
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { ConcurrencyStamp = "5e00eea4-faa4-4a00-97eb-4c3e9041aa8e", Id = "5e00eea4-faa4-4a00-97eb-4c3e9041aa5c", Name = "admin", NormalizedName = "admin".ToUpper() },
                new IdentityRole { ConcurrencyStamp = "5e00eea4-faa4-4a00-97eb-4c3e9041aa8c", Id = "5e00eea4-faa4-4a00-97eb-4c3e9941aa8c", Name = "user", NormalizedName = "user".ToUpper() }

                );



            base.OnModelCreating(modelBuilder);
        }
    }
}

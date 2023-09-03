using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.EntityTypeConfiguration
{
    public class ProductImageTC : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi=>pi.Id);
            builder.Property(pi=>pi.Name).IsRequired();
            builder
                .HasOne(pi => pi.product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

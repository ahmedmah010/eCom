using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.EntityTypeConfiguration
{
    public class TagTC : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder) 
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired();

            builder
                .HasMany(t => t.Products)
                .WithMany(p => p.Tags)
                .UsingEntity<ProductTag>(
                j=>j
                    .HasOne(j => j.Product)
                    .WithMany(p=>p.ProdTag)
                    .HasForeignKey(j => j.ProductId)
                    .HasPrincipalKey(p=>p.Id),
                j=>j
                    .HasOne(j=>j.Tag)
                    .WithMany(t=>t.ProdTag)
                    .HasForeignKey(j=>j.TagId)
                    .HasPrincipalKey(t=>t.Id),
                j=>j
                    .HasKey(j=> new {j.ProductId,j.TagId})
                );
        }
    }
}

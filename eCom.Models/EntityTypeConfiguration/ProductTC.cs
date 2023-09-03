using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.EntityTypeConfiguration
{
    public class ProductTC : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id); 
            builder.Property(p=>p.Title).IsRequired();
            builder.Property(p=>p.Brand).IsRequired();
            builder.Property(p => p.CurrentQuantity).IsRequired();
            builder.Property(p => p.CurrentPrice).IsRequired();
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.CategoryId).IsRequired();
        }
    }
}

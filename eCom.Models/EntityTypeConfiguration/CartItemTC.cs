using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.EntityTypeConfiguration
{
    public class CartItemTC : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.
                HasOne(c => c.Product)
               .WithOne()
               .HasForeignKey<CartItem>(c=>c.ProductId)
               .HasPrincipalKey<Product>(p=>p.Id);
               
        }
    }
}

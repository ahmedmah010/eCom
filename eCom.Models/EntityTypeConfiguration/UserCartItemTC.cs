using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.EntityTypeConfiguration
{
    public class UserCartItemTC : IEntityTypeConfiguration<UserCartItem>
    {
        public void Configure(EntityTypeBuilder<UserCartItem> builder)
        {
            builder.HasKey(obj => obj.Id); //Primary Key

            builder
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId)
                .HasPrincipalKey(u => u.Id);
                
        }
    }
}

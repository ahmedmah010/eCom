using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eCom.Models.EntityTypeConfiguration
{
    public class UserAddressTC : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .HasOne<AppUser>()
                .WithMany(u=>u.Addresses)
                .HasForeignKey(address=>address.UserId)
                .HasPrincipalKey(u=>u.Id);

        }
    }
}

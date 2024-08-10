using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace eCom.Models
{
    public class AppUser : IdentityUser
    {
        public string Fname { get; set; }
        public string Lname { get; set; }

        //Nagivation Propery
        virtual public List<UserAddress> Addresses { get; set; }

        public virtual List<UserCartItem>? CartItems { get; set; }

        public virtual ICollection<ProductComment>? ProductsComments { get; set; } = new HashSet<ProductComment>();

 
    }
   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class UserCartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }

        public string UserId { get; set; }
    
       public virtual AppUser User { get; set; }
       
    }
}

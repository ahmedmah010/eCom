using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        //ForiegnKey
        public int ProductId { get; set; } 

        //Navigation Property
        public virtual Product Product { get; set; }
        

    }
}

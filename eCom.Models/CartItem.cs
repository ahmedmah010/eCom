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
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Img { get; set; }
        public int ProductPrice { get; set; }
        public int ProductId { get; set; }
        public int SubTotal { get; set; }

        public static int CreatedCartItems;
        public static int TotalPrice=0;

        public CartItem()
        {
            CreatedCartItems++;
            this.Id = CreatedCartItems;
        }
        ~CartItem()
        {
            --CreatedCartItems;
        } 


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class OrderItemVM
    {
        public int? ProductId { get; set; }
        public float Price { get; set; }
        public string ProductTitle { get; set; }
        public string ProductImage { get; set; }
        public string ProductBrand { get; set; }
        public int Quantity { get; set; }
        public float SubTotal => Price * Quantity;
    }
}

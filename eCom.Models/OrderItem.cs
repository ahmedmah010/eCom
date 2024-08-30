using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int? ProductId {  get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        //Why duplicate Product properties instead of using the relationship navigation property? Well, What if the product is deleted from the DB? then it will affect the OrderItem and it will be gone from here. So, I've to explictly store the data + I may use the relationship to allow the user to click on the item to go the product page if it exists, if it doesn't it will show a page with product not found
        public float Price { get; set; }
        public string ProductTitle { get; set; }
        public byte[] ProductImage { get; set; }
        public string ProductBrand { get; set; }
        public int  Quantity {  get; set; }

        //Navigation Properties 
        public virtual Product? Product { get; set; }
        public virtual Order Order { get; set; }

    }
}

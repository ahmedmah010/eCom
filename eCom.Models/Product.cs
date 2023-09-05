
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCom.Models
{
    public class Product
    {
        
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Brand { get; set; }
        
        public int CurrentQuantity { get; set; }
        
        public int CurrentPrice { get; set; }

        public int OldPrice { get; set; }

        public int? Discount { get; set; } = 0;
        
        public string? Description { get; set; }
        
        public int CategoryId { get; set; }
        //Navigation Property
        [ValidateNever]
        public virtual Category Category { get; set; }
        //Skip Navigation 
        public virtual IList<Tag>? Tags { get; set; }
        //Navigation Property
        public virtual List<ProductTag>? ProdTag { get; set; }
        //Navigation Property
        public virtual List<ProductImage> Images { get; set; }

    }
}

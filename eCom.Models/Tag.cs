using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class Tag
    {
        
        public int Id { get; set; }
        
        public string? Name { get; set; }

        public virtual IList<Product>? Products { get; set; }

        public virtual List<ProductTag>? ProdTag { get; set; }
    }
}

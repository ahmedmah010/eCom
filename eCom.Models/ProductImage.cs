using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int ProductId { get; set; }
        public virtual Product product { get; set; }
    }
}

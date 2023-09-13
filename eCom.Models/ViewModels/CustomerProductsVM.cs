using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class CustomerProductsVM
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set;}

        public int PaginationButtons { get; set; }
    }
}

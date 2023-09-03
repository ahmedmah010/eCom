using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class Category
    {
        public int Id { get; set; }
      
        public string? Name { get; set; }

        [ValidateNever]
        public virtual List<Product>? Products { get; set; }
    }
}

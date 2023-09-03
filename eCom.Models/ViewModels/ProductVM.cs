using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public int CurrentQuantity { get; set; }
        [Required]
        public int CurrentPrice { get; set; }
        public int OldPrice { get; set; } = 0;

        public int? Discount { get; set; }

        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        [Required]
        public string? Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<SelectListItem>? CategoriesList { get; set; }
        public List<SelectListItem>? TagsList { get; set; }

        [Required]
        public List<string> SelectedTags { get; set; } = new List<string>();

    
    }
}

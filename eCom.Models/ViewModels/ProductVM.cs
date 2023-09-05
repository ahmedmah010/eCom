using Microsoft.AspNetCore.Mvc;
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
        [Required, MinLength(10, ErrorMessage = "Min length is 10 letters"), MaxLength(50, ErrorMessage = "Max length is 50 letters")]
        public string Title { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required, Range(1,5000, ErrorMessage = "Range must be between 1 and 5000")]
        public int CurrentQuantity { get; set; }
        public int CurrentPrice { get; set; }
        [Required]
        public int OldPrice { get; set; }

        public int? Discount { get; set; } = 0;
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        [Required]
        public string? Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<SelectListItem>? CategoriesList { get; set; }
        public List<SelectListItem>? TagsList { get; set; }
        public List<string> SelectedTags { get; set; } = new List<string>();

    
    }
}

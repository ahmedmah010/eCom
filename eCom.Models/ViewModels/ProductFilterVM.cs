using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class ProductFilterVM
    {
        public string? Display { get; set; } = "3";
        public string? SortBy { get; set; } = "0";
        public List<BrandCheckBox>? Brands { get; set; }
        public string? Category { get; set; }
        public string? PriceFrom { get; set; }
        public string? PriceTo { get; set; }
    }
    public class BrandCheckBox
    {
        public string Value { get; set;}
        public bool IsChecked { get; set;}
    }
}

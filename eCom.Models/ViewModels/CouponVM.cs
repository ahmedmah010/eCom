using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class CouponVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; } // e.g., "Percentage", "FixedAmount"
        public float DiscountValue { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UsageLimit { get; set; }
        public float MinPurchaseAmount { get; set; }
        public float MaxDiscountAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsSingleUse { get; set; }
        public List<Category>? ApplicableCategories { get; set; } = new List<Category>();
        public List<int>? SelectedCatIds { get; set; } = new List<int>();
        public List<Category>? Categories { get; set; } = new List<Category>();
    }

 
}

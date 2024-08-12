using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; } // e.g., "Percentage", "FixedAmount"
        public decimal DiscountValue { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public decimal MinPurchaseAmount { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsSingleUse { get; set; }
        public virtual ICollection<Product> ApplicableProducts { get; set; } = new HashSet<Product>();
        public virtual ICollection<Category> ApplicableCategories { get; set; } = new HashSet<Category>();
    }

}

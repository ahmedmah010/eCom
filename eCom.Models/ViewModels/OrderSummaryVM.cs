using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class OrderSummaryVM
    {
        public List<Tax>? AppliedTaxes { get; set; } = new List<Tax>();
        public float TotalPriceAfter { get; set; } = 0;
        public float TotalPriceBefore { get; set; } = 0;
        public float Discount { get; set; } = 0;
        public float DeliveryFees { get; set; } = 0;
    }
}

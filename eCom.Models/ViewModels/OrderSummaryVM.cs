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
        public float TotalPrice { get; set; }
        public float Discount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TaxType TaxType {  get; set; }
        public float Amount { get; set; }
        
    }
    public enum TaxType
    {
        Percentage, FixedAmount
    }
}

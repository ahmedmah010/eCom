using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxType {  get; set; }
        public float Amount { get; set; }
        
    }
    [NotMapped]
    public static class TaxType
    {
        public const string Percentage = "Percentage";
        public const string FixedAmount = "Fixed Amount";
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    [NotMapped]
    public class OrderVM
    {
        public string PaymentMethod { get; set; }
        public string? Coupon { get; set; }
        public List<UserAddress> Addresses { get; set; }
        public int ChosenAddressId { get; set; }
        public ICollection<OrderItemVM> OrderItemsVM { get; set; } = new HashSet<OrderItemVM>();
    }
}

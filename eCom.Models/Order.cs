using eCom.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = OrderStatus.Pending;
        public string PaymentMethod { get; set; }
        [ForeignKey("User")]
        public string UserId {  get; set; }
        public Address Address { get; set; }
        public float TotalPrice {  get; set; }
        public float DeliveryFees {  get; set; }
        public float Discount {  get; set; }
        public float TotalTaxes {  get; set; }

        //Navigation Properties
        public virtual AppUser User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        


    }

    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Accepted = "Accepted";
        public const string Delivered = "Delivered";
        public const string Rejected = "Rejected";
    }
    public static class PaymentMethod
    {
        public const string COD = "COD";
        public const string VISA = "VISA";
    }
    [NotMapped]
    [Owned]
    public class Address
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string NearestLandMark { get; set; }
        public string AdditionalInfo { get; set; }
        public string City { get; set; }
    }
}

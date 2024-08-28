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
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        [ForeignKey("User")]
        public string UserId {  get; set; }
        public string? CouponName {  get; set; }
        public float? CouponValue {  get; set; }
        public Address Address { get; set; }
        public float TotalPrice {  get; set; }

        //Navigation Properties
        public virtual AppUser User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        


    }

    public enum OrderStatus
    {
        Pending, Accepted, Delivered, Rejected
    }
    public enum PaymentMethod
    {
        COD, VISA
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

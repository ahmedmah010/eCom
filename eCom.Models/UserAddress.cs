using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string NearestLandMark { get; set; }
        public string AdditionalInfo { get; set; }
        public string UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Street { get; set; }
        public string NearestLandMark { get; set; }
        public string AdditionalInfo { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }
        public bool IsDefault {get; set; }

        //Navigation Properties
        public virtual AppUser User { get; set; }
        public virtual City City { get; set; }

    }
}

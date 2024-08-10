using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models
{
    public class ProductComment
    {
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 2)]
        public string? Description { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string? Title { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Range(1,5)]
        public int Rating { get; set; }

        public bool VerifiedPurchase { get; set; } = false;

        //Navigation Properties
        public virtual AppUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    [NotMapped]
    public class ProductCommentVM
    {
        public int Id { get; set; }
        [Range(1,5)]
        public int Rating { get; set; }
        [StringLength(50,MinimumLength =2)]
        public string? Title { get; set; }
        [StringLength(200, MinimumLength = 2)]
        public string? Description { get; set; }
    }
}

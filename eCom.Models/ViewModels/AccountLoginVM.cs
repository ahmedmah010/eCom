using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class AccountLoginVM
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remeber Me")]
        public bool RememberMe { get; set; }
    }
}

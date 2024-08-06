using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class AccountRegisterVM
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Name must be between 3 to 10 characters.")]
        [Display(Name = "First Name")]
        public string Fname { get; set; }
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Name must be between 3 to 10 characters.")]
        [Display(Name = "Last Name")]
        public string Lname { get; set; }
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}

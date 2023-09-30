using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class AccountRegisterVM
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}

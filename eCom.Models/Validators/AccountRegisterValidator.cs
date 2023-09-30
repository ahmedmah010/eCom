using eCom.Models.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.Validators
{
    public class AccountRegisterValidator : AbstractValidator<AccountRegisterVM>
    {
        public AccountRegisterValidator(UserManager<AppUser> _userManager)
        {
            RuleFor(e=>e.UserName).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Required")
                .MustAsync(async (username, cancellation) =>
                {
                        return await _userManager.FindByNameAsync(username) == null;      
                }).WithMessage("{PropertyValue} is already taken");

            RuleFor(e=>e.Email).NotEmpty().WithMessage("Required");
            MakeRequired(e => e.Fname);
            MakeRequired(e => e.Lname);
            MakeRequired(e => e.Password);
            RuleFor(e => e.ConfirmPassword).NotEmpty().WithMessage("Required").Matches(e => e.Password).WithMessage("Passwords Must Match");
        }
        private void MakeRequired<T>(Expression<Func<AccountRegisterVM,T>> exp)
        {
            RuleFor(exp).NotEmpty().WithMessage("Required");
        }
    }
}

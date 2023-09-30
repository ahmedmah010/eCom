﻿using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.Validators;
using eCom.Models.ViewModels;
using eCom.Utilities;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using FormHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IValidator<AccountRegisterVM> _regValidator;
        private readonly IRepo<UserAddress> _userAdrsRepo;
        public AccountController(UserManager<AppUser> um,
                                IValidator<AccountRegisterVM> regValidator, 
                                SignInManager<AppUser> signInManager,
                                IRepo<UserAddress> userAdrs
                                )
        {
            _userManager = um;
            _regValidator = regValidator;
            _signInManager = signInManager;
            _userAdrsRepo = userAdrs;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]AccountRegisterVM _regVM)
        {
            ValidationResult _regValidationResult = await _regValidator.ValidateAsync(_regVM); 
            if (ModelState.IsValid)
            {
                if(!_regValidationResult.IsValid)
                {
                    _regValidationResult.AddToModelState(ModelState);
                }
                else
                {
                    AppUser _newUser = new AppUser();
                    _newUser.UserName = _regVM.UserName;
                    _newUser.Email = _regVM.Email;
                    _newUser.Fname = _regVM.Fname;
                    _newUser.Lname = _regVM.Lname;
                    IdentityResult _regResult = await _userManager.CreateAsync(_newUser, _regVM.Password);
                    if (_regResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(_newUser, Role.User);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach(var err in _regResult.Errors) 
                        {
                            Console.WriteLine(err.Code);
                            ModelState.AddModelError("", err.Description);
                        }
                    }
                }
            }
            return View(_regVM);
        }
        public IActionResult Login([FromQuery]string ReturnUrl=null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginVM _loginVM)
        {
            if(ModelState.IsValid)
            {
                AppUser _user = await _userManager.FindByNameAsync(_loginVM.UserName);
                if( _user == null )
                {
                    ModelState.AddModelError("UserName", "username is not found");
                }
                else if(!(await _userManager.CheckPasswordAsync(_user, _loginVM.Password)))
                {
                    ModelState.AddModelError("Password", "wrong password");
                }
                else
                {
                    AuthenticationProperties _authProperties = new AuthenticationProperties();
                    if (_loginVM.RememberMe)
                    {
                        _authProperties.IsPersistent = true;
                        _authProperties.ExpiresUtc = DateTime.Now.AddMonths(1);
                    }
                    else
                    {
                        _authProperties.IsPersistent = false;
                    }
                    await _signInManager.SignInAsync(_user, _authProperties);
                    if (TempData["ReturnUrl"]!=null)
                    {
                        string _url = TempData["ReturnUrl"] as string;
                        return LocalRedirect(_url);
                    }
                    return RedirectToAction("Index","Home");
                }
            }

            return View(_loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            if(User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index","Home");
        }
        [Authorize]
        public async Task<IActionResult> Address()
        {
            AppUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            return View(_user.Addresses);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NewAddress(UserAddress _userAdrs)
        {
            _userAdrs.UserId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            ModelState.Remove("UserId");
            ModelState.SetModelValue("UserId", new ValueProviderResult(_userAdrs.UserId));
            ModelState.MarkFieldValid("UserId");
            if (ModelState.IsValid)
            {
                _userAdrsRepo.add(_userAdrs);
                _userAdrsRepo.SaveChanges();
            }
            return RedirectToAction("Address","Account");
        }
        [Authorize]
        public IActionResult EditAddress(int Id)
        {
            UserAddress _userAddress = new UserAddress();
            if(Id>0)
            {
               _userAddress = _userAdrsRepo.Get(a => a.Id == Id);
            }
            return PartialView("~/Views/Shared/Customer_UserAddressPV/_EditAddress.cshtml", _userAddress);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditAddress(UserAddress _userAddress)
        {
            if (ModelState.IsValid)
            {
                string _currUserId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                if(_currUserId == _userAddress.UserId)
                {
                    _userAdrsRepo.update(_userAddress);
                    _userAdrsRepo.SaveChanges();
                }
            }
            return RedirectToAction("Address","Account");
        }
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int Id)
        {
            if (Id>0)
            {
                UserAddress _targetAdrs = _userAdrsRepo.Get(a=>a.Id==Id);
                if(_targetAdrs!=null)
                {
                    string _currUserId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                    if (_currUserId == _targetAdrs.UserId)
                    {
                        _userAdrsRepo.remove(_targetAdrs);
                        _userAdrsRepo.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Address", "Account");
        }
    }

}

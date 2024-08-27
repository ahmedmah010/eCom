using AutoMapper;
using eCom.DataAccess;
using eCom.DataAccess.Data;
using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using FormHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        //private readonly IValidator<AccountRegisterVM> _regValidator; Used for FLUENT VALIDATION LIB
        private readonly IRepo<UserAddress> _userAdrsRepo;
        private readonly IRepo<AppUser> _appUserRepo;
        private readonly IRepo<City> _cityRepo;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> um,
                                SignInManager<AppUser> signInManager,
                                IRepo<UserAddress> userAdrs,
                                IRepo<AppUser> appUserRepo,
                                IRepo<City> cityRepo,
                                IMapper mapper

                                )
        {
            _userManager = um;
            _signInManager = signInManager;
            _userAdrsRepo = userAdrs;
            _appUserRepo = appUserRepo;
            _cityRepo = cityRepo;
            _mapper = mapper;   
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]AccountRegisterVM _regVM)
        {
            if (ModelState.IsValid)
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
                        TempData["NewReg"] = "true";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach(var err in _regResult.Errors) 
                        {
                            ModelState.AddModelError("", err.Description);
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
                    ModelState.AddModelError("UserName", "Username is not found");
                }
                else if(!(await _userManager.CheckPasswordAsync(_user, _loginVM.Password)))
                {
                    ModelState.AddModelError("Password", "Wrong password");
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
                    await _signInManager.SignInAsync(_user, _authProperties); //creates cookie aka logs in
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
                await _signInManager.SignOutAsync(); //removes the cookie
            }
            return RedirectToAction("Index","Home");
        }

        /* Address Logic */


        [Authorize]
        public IActionResult Address()
        {
            AppUser _user =  _userManager.GetUserAsync(User).Result;
            
            return View(_user.Addresses);
        }

        [Authorize]
        public IActionResult UpsertAddress(int Id)
        {
            SelectList CitiesSelectList = new SelectList(_cityRepo.GetAll(),"Id","Name");
            ViewBag.Cities = CitiesSelectList;
            UserAddressVM userAddressVM = new UserAddressVM();
            if(Id <=0 )
            {
                return PartialView("~/Views/Shared/Customer_UserAddressPV/_UpsertAddress.cshtml", userAddressVM);
            }
       
            else
            {
                UserAddress _userAddress = _userAdrsRepo.Get(a => a.Id == Id);
                if (_userAddress != null)
                {

                    userAddressVM = _mapper.Map<UserAddressVM>(_userAddress);

                }
                return PartialView("~/Views/Shared/Customer_UserAddressPV/_UpsertAddress.cshtml", userAddressVM);
            }
            
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpsertAddress(UserAddressVM _userAddress)
        {

            if (ModelState.IsValid)
            {
                UserAddress useraddrs = new UserAddress();
                useraddrs = _mapper.Map<UserAddress>(_userAddress);
                useraddrs.UserId = _userManager.GetUserId(User);

                if (useraddrs.Id==0) //new address
                {
                    _userAdrsRepo.add(useraddrs);
                }
                else //update
                {
                    _userAdrsRepo.update(useraddrs);
                }
                _userAdrsRepo.SaveChanges();
                return Content("");
            }
            else
            {
                return PartialView("~/Views/Shared/Customer_UserAddressPV/_UpsertAddress.cshtml",_userAddress);
            }
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

        [Authorize]
        public async Task<IActionResult> DeleteAccount() //for debugging purposes
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _signInManager.SignOutAsync(); //To delete the authenticaion cookie
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index", "Home");
        }


    }

}


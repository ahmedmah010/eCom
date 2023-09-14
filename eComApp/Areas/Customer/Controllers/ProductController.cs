using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class ProductController : Controller
    {
        private readonly IRepo<Product> _ProductRepo;
        private readonly IRepo<Category> _CategoryRepo;
        private const int PageSize = 3; 
        public ProductController(IRepo<Product> Prod, IRepo<Category> Cat)
        {
            _ProductRepo = Prod;
            _CategoryRepo = Cat;
        }
        public IActionResult Index()
        {
            CustomerProductsVM CustomerProductsVM = new CustomerProductsVM();
            CustomerProductsVM.Categories = _CategoryRepo.GetAll().ToList();
            CustomerProductsVM.ProductFilterVM.Brands = new List<BrandCheckBox> { 
                new BrandCheckBox {Value="Apple",IsChecked=false},
                new BrandCheckBox {Value="Samsung",IsChecked=false},
            };
            CustomerProductsVM.Products = _ProductRepo.GetAll().ToList();
            CustomerProductsVM.PaginationButtons = (int)Math.Ceiling((double)_ProductRepo.Count()/ (double)PageSize);
            return View(CustomerProductsVM);
        }
        public IActionResult PaginatePV()
        {
            return PartialView("~/Views/Shared/Customer_ProductPV/_ProductPV.cshtml",_ProductRepo.GetAll().ToList());
        }
        public IActionResult Paginate(int PageNumber)
        {
            return RedirectToAction("Index", new {PageNumber=PageNumber});
        }
        public IActionResult ProductFilter(ProductFilterVM ProductFilterVM, List<string>Brands)
        {
            CustomerProductsVM CustomerProductsVM = new CustomerProductsVM();
            CustomerProductsVM.Products = _ProductRepo.GetAll().ToList();
            CustomerProductsVM.ProductFilterVM = ProductFilterVM;
            CustomerProductsVM.Categories = _CategoryRepo.GetAll().ToList();
            ProductFilterVM.Brands = new List<BrandCheckBox> {
                new BrandCheckBox {Value="Apple",IsChecked=false},
                new BrandCheckBox {Value="Samsung",IsChecked=false},
            };
            bool IsFiltered = false;
            if (!Brands.IsNullOrEmpty())
            {
                IsFiltered = true;
                CustomerProductsVM.Products = CustomerProductsVM.Products.Where(p=>Brands.Contains(p.Brand)).ToList();
                foreach(var CheckBoxBrand in ProductFilterVM.Brands) //for UI purposes only (Make checkbox checked automatically)
                {
                    if(Brands.Contains(CheckBoxBrand.Value))
                    {
                        CheckBoxBrand.IsChecked = true;
                    }
                }
            }
            if(!String.IsNullOrEmpty(ProductFilterVM.Category))
            {
                IsFiltered = true;
                CustomerProductsVM.Products = CustomerProductsVM.Products.Where(p=>p.Category.Name==ProductFilterVM.Category).ToList();
            }
            return IsFiltered?View("Index", CustomerProductsVM):RedirectToAction("Index");
        }
    }
}

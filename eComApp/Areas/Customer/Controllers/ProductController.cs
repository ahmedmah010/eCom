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
            TempData["FilteredProducts"] = null;
            CustomerProductsVM CustomerProductsVM = new CustomerProductsVM();
            CustomerProductsVM.Categories = _CategoryRepo.GetAll().ToList();
            CustomerProductsVM.ProductFilterVM.Brands = new List<BrandCheckBox> { 
                new BrandCheckBox {Value="Apple",IsChecked=false},
                new BrandCheckBox {Value="Samsung",IsChecked=false},
            };
            CustomerProductsVM.Products = _ProductRepo.GetAll().ToList();
            CustomerProductsVM.Products = CustomerProductsVM.Products.Skip(0).Take(PageSize).ToList();
            CustomerProductsVM.PaginationButtons = (int)Math.Ceiling((double)_ProductRepo.Count()/ (double)PageSize);
            return View(CustomerProductsVM);
        }
       
        public IActionResult Paginate(int PageNumber)
        {
            CustomerProductsVM CustomerProductsVM = new CustomerProductsVM();
            CustomerProductsVM.Categories = _CategoryRepo.GetAll().ToList();
            CustomerProductsVM.ProductFilterVM.Brands = new List<BrandCheckBox> {
                new BrandCheckBox {Value="Apple",IsChecked=false},
                new BrandCheckBox {Value="Samsung",IsChecked=false},
            };
            if (TempData["FilteredProducts"]!=null)
            {
                CustomerProductsVM.Products = (List<Product>)TempData.Peek("FilteredProducts");
            }
            else
            {
                CustomerProductsVM.Products = _ProductRepo.GetAll().ToList();
            }
            CustomerProductsVM.Products = CustomerProductsVM.Products.Skip((PageNumber-1)*PageSize).Take(PageSize).ToList();
            return View("Index", CustomerProductsVM);
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
            bool IsFiltered = false; //Helps to handle the case if a user hardcoded filters in the URL
            if (!Brands.IsNullOrEmpty())
            {
                IsFiltered = true;
                CustomerProductsVM.Products = CustomerProductsVM.Products.Where(p => Brands.Contains(p.Brand)).ToList();
                foreach (var CheckBoxBrand in ProductFilterVM.Brands) //for UI purposes only (Make checkbox checked automatically)
                {
                    if (Brands.Contains(CheckBoxBrand.Value))
                    {
                        CheckBoxBrand.IsChecked = true;
                    }
                }
            }
            if (!String.IsNullOrEmpty(ProductFilterVM.Category))
            {
                IsFiltered = true;
                CustomerProductsVM.Products = CustomerProductsVM.Products.Where(p => p.Category.Name == ProductFilterVM.Category).ToList();
            }
            if (IsFiltered)
            {
                CustomerProductsVM.PaginationButtons = (int)Math.Ceiling((double)CustomerProductsVM.Products.Count / (double)PageSize);
                TempData["FilteredProducts"] = CustomerProductsVM.Products;
                return View("Index", CustomerProductsVM);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}

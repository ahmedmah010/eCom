using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using eCom.Utilities.ExtentionMethods;
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


        /* *************************************************** Helper Methods *************************************************** */

        private void InitializeCustomerProductVM(CustomerProductsVM _vm)
        {
            _vm.Categories = _CategoryRepo.GetAll().ToList();
            _vm.ProductFilterVM.BrandsCheckBox = new List<BrandCheckBox> {
                new BrandCheckBox {Value="Apple",IsChecked=false},
                new BrandCheckBox {Value="Samsung",IsChecked=false},
            };
            _vm.Products = _ProductRepo.GetAll().ToList();
        }
        private void ProductsPaginationAndButtonsPagination(CustomerProductsVM _vm, int PageNumber=1)
        {
            _vm.PaginationButtons = (int)Math.Ceiling((double)_vm.Products.Count() / (double)PageSize);
            _vm.Products = _vm.Products.Skip((PageNumber-1)*PageSize).Take(PageSize).ToList();
        }


        /* *************************************************** End Helper Methods *************************************************** */



        public IActionResult Index(int? PageNumber, ProductFilterVM _ProductFilterVM)
        {
            CustomerProductsVM _CustomerProductsVM = new CustomerProductsVM();
            _CustomerProductsVM.ProductFilterVM = _ProductFilterVM;
            InitializeCustomerProductVM(_CustomerProductsVM);
            if (_ProductFilterVM.GetType().GetProperties().Any(prop=> prop.Name!="BrandsCheckBox" && prop.GetValue(_ProductFilterVM) !=null)) //Prevent unnecessary checks if there's no filters
            {
                ProductFilter(_CustomerProductsVM);
            }
            if (PageNumber!=null && PageNumber!=0 && PageNumber>1)
            {
                ProductsPaginationAndButtonsPagination(_CustomerProductsVM, PageNumber.Value);
            }
            else
            {

                ProductsPaginationAndButtonsPagination(_CustomerProductsVM);
            }
          
            return View(_CustomerProductsVM);
        }


        private void ProductFilter(CustomerProductsVM _CustomerProductsVM)
        {
            if (!_CustomerProductsVM.ProductFilterVM.Brands.IsNullOrEmpty())
            {
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p => _CustomerProductsVM.ProductFilterVM.Brands.Contains(p.Brand)).ToList();
                foreach (var CheckBoxBrand in _CustomerProductsVM.ProductFilterVM.BrandsCheckBox) //for UI purposes only (Make checkbox checked automatically)
                {
                    if (_CustomerProductsVM.ProductFilterVM.Brands.Contains(CheckBoxBrand.Value))
                    {
                        CheckBoxBrand.IsChecked = true;
                    }
                }
            }
            if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.Category) && _CustomerProductsVM.ProductFilterVM.Category!="All")
            {
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p => p.Category.Name == _CustomerProductsVM.ProductFilterVM.Category).ToList();
            }
            if(!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceFrom) || !String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceTo))
            {
                int TempFrom=0, TempTo=0;
                if(!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceFrom) && int.Parse(_CustomerProductsVM.ProductFilterVM.PriceFrom)>0)
                {
                    TempFrom = int.Parse(_CustomerProductsVM.ProductFilterVM.PriceFrom);
                }
                if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceTo) && int.Parse(_CustomerProductsVM.ProductFilterVM.PriceTo) > 0)
                {
                    TempTo = int.Parse(_CustomerProductsVM.ProductFilterVM.PriceTo);
                }
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p => p.CurrentPrice >= TempFrom && p.CurrentPrice <= TempTo).ToList();
            }
            if(_CustomerProductsVM.ProductFilterVM.SortBy!="0")
            {
                if(_CustomerProductsVM.ProductFilterVM.SortBy == "asc")
                {
                    _CustomerProductsVM.Products.Sort((a,b)=>a.CurrentPrice.CompareTo(b.CurrentPrice));
                }
                else if(_CustomerProductsVM.ProductFilterVM.SortBy == "desc")
                {
                    _CustomerProductsVM.Products.Sort((a,b)=>b.CurrentPrice.CompareTo(a.CurrentPrice));
                }
            }
        }
    }
}









//public IActionResult ProductFilter(ProductFilterVM ProductFilterVM)
//{
//    CustomerProductsVM CustomerProductsVM = new CustomerProductsVM();
//    CustomerProductsVM.ProductFilterVM = ProductFilterVM;
//    InitializeCustomerProductVM(CustomerProductsVM);
//    bool IsFiltered = false; //Helps to handle the case if a user hardcoded wrong filters in the URL and to handle TempData+pagination
//    if (!ProductFilterVM.Brands.IsNullOrEmpty())
//    {
//        IsFiltered = true;
//        CustomerProductsVM.Products = CustomerProductsVM.Products.Where(p => ProductFilterVM.Brands.Contains(p.Brand)).ToList();
//        foreach (var CheckBoxBrand in ProductFilterVM.BrandsCheckBox) //for UI purposes only (Make checkbox checked automatically)
//        {
//            if (ProductFilterVM.Brands.Contains(CheckBoxBrand.Value))
//            {
//                CheckBoxBrand.IsChecked = true;
//            }
//        }
//    }
//    if (!String.IsNullOrEmpty(ProductFilterVM.Category))
//    {
//        IsFiltered = true;
//        CustomerProductsVM.Products = CustomerProductsVM.Products.Where(p => p.Category.Name == ProductFilterVM.Category).ToList();
//    }
//    if (IsFiltered)
//    {
//        List<int> ProductIDs = CustomerProductsVM.Products.Select(p => p.Id).ToList();
//        TempData.Push("FilteredProductsIDs", ProductIDs);
//        ProductsPaginationAndButtonsPagination(CustomerProductsVM, 1);
//        return View("Index", CustomerProductsVM);
//    }
//    else
//    {
//        return RedirectToAction("Index");
//    }
//}
//    }
//}

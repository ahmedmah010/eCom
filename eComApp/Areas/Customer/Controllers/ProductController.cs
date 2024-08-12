using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using eCom.Utilities.ExtentionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class ProductController : Controller
    {
        private readonly IRepo<Product> _ProductRepo;
        private readonly IRepo<Category> _CategoryRepo;
        private readonly IRepo<ProductComment> _ProductCommentRepo;
        private readonly UserManager<AppUser> _UserManager;
        private int PageSize = 3; 
        public ProductController(IRepo<Product> Prod, IRepo<Category> Cat, IRepo<ProductComment> _prodComment, UserManager<AppUser> _usermang)
        {
            _ProductRepo = Prod;
            _CategoryRepo = Cat;
            _ProductCommentRepo = _prodComment;
            _UserManager = _usermang;
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


        private void ProductFilter(CustomerProductsVM _CustomerProductsVM)
        {
            bool isFiltered = false;

            if (!_CustomerProductsVM.ProductFilterVM.Brands.IsNullOrEmpty())
            {
                isFiltered = true;
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p => _CustomerProductsVM.ProductFilterVM.Brands.Contains(p.Brand)).ToList();
                foreach (var CheckBoxBrand in _CustomerProductsVM.ProductFilterVM.BrandsCheckBox) //for UI purposes only (Make checkbox checked automatically)
                {
                    if (_CustomerProductsVM.ProductFilterVM.Brands.Contains(CheckBoxBrand.Value))
                    {
                        CheckBoxBrand.IsChecked = true;
                    }
                }
            }
            if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.Category) && _CustomerProductsVM.ProductFilterVM.Category != "All")
            {
                isFiltered = true;
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p => p.Category.Name == _CustomerProductsVM.ProductFilterVM.Category).ToList();
            }
            if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceFrom) || !String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceTo))
            {
                int TempFrom = 0, TempTo = 0;
                if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceFrom) && int.Parse(_CustomerProductsVM.ProductFilterVM.PriceFrom) > 0)
                {
                    TempFrom = int.Parse(_CustomerProductsVM.ProductFilterVM.PriceFrom);
                    isFiltered = true;
                }
                if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.PriceTo) && int.Parse(_CustomerProductsVM.ProductFilterVM.PriceTo) > 0)
                {
                    TempTo = int.Parse(_CustomerProductsVM.ProductFilterVM.PriceTo);
                    isFiltered = true;
                }
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p => p.CurrentPrice >= TempFrom && p.CurrentPrice <= TempTo).ToList();
            }
            if (_CustomerProductsVM.ProductFilterVM.SortBy != "0")
            {
                if (_CustomerProductsVM.ProductFilterVM.SortBy == "asc")
                {
                    isFiltered = true;
                    _CustomerProductsVM.Products.Sort((a, b) => a.CurrentPrice.CompareTo(b.CurrentPrice));
                }
                else if (_CustomerProductsVM.ProductFilterVM.SortBy == "desc")
                {
                    isFiltered = true;
                    _CustomerProductsVM.Products.Sort((a, b) => b.CurrentPrice.CompareTo(a.CurrentPrice));
                }
            }
            if (!String.IsNullOrEmpty(_CustomerProductsVM.ProductFilterVM.Display))
            {
                isFiltered = true;
                switch (_CustomerProductsVM.ProductFilterVM.Display)
                {
                    case "1":
                        PageSize = 1;
                        break;
                    case "2":
                        PageSize = 2;
                        break;
                    case "3":
                        PageSize = 3;
                        break;
                }
            }
            if (isFiltered)
            {
                TempData["FilteredProductsIDs"] = null;
                List<int> ProductIDs = _CustomerProductsVM.Products.Select(p => p.Id).ToList();
                TempData.Push("FilteredProductsIDs", ProductIDs);
            }
            else
            {
                TempData["FilteredProductsIDs"] = null;
            }
        }

        /* *************************************************** End Helper Methods *************************************************** */



        public IActionResult Index(int? PageNumber, ProductFilterVM _ProductFilterVM) //Entry Point
        {
            CustomerProductsVM _CustomerProductsVM = new CustomerProductsVM();
            _CustomerProductsVM.ProductFilterVM = _ProductFilterVM;
            InitializeCustomerProductVM(_CustomerProductsVM);
            ProductFilter(_CustomerProductsVM);
            if (PageNumber == null)
            {
                ProductFilter(_CustomerProductsVM);
            }
            else if (TempData["FilteredProductsIDs"] != null && PageNumber >= 1)
            {
                List<int> ProductIDs = TempData.Get<List<int>>("FilteredProductsIDs");
                _CustomerProductsVM.Products = _CustomerProductsVM.Products.Where(p=>ProductIDs.Contains(p.Id)).ToList();
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


        public IActionResult ProductSearchPV(string text)
        {
            List<ProductSearchVM> SearchedProducts = new List<ProductSearchVM>();
            if(!String.IsNullOrWhiteSpace(text) && text.Length>=3)
            {
                SearchedProducts = _ProductRepo.Where(p=>p.Title.Contains(text) || p.Brand.Contains(text)).Select(p=>new ProductSearchVM { Id=p.Id,Name=p.Title,Brand=p.Brand}).ToList();
            }
            if(SearchedProducts.Count == 0)
            {
                return Content("");
            }
            return PartialView("~/Views/Shared/Customer_ProductPV/_ProductSearch.cshtml", SearchedProducts);
        }

        /*product details*/
        public IActionResult ProductDetails(int id)
        {
            Product? prd = _ProductRepo.Get(prd => prd.Id == id);
            if (prd != null)
            {
                return View(prd);
            }
            return RedirectToAction("Index", "Home");

        }
        [Authorize]
        public IActionResult UpsertProdComment(int Id)
        {
            ProductCommentVM productCommentVM = new ProductCommentVM();
            if (Id == 0) //New Comment
            {
                return PartialView("_UpsertProdComment", productCommentVM);
            }
            else //edit
            {
                ProductComment prodComment = _ProductCommentRepo.Get(comment=>comment.Id == Id);
                productCommentVM.Id = prodComment.Id;
                productCommentVM.Title = prodComment.Title;
                productCommentVM.Description = prodComment.Description;
                productCommentVM.Rating = prodComment.Rating;
                return PartialView("_UpsertProdComment", productCommentVM);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpsertProdComment(ProductCommentVM prodCommentVM, int prodId)
        {
            //prodid, commentid
            if (ModelState.IsValid)
            {
                if (prodCommentVM.Id == 0) //new comment
                {
                    ProductComment prodComment = new ProductComment
                    {
                        Title = prodCommentVM.Title,
                        Description = prodCommentVM.Description,
                        Rating = prodCommentVM.Rating,
                        ProdId = prodId,
                        UserId = _UserManager.GetUserId(User)
                    };
                    _ProductCommentRepo.add(prodComment);
                }
                else //update
                {
                    ProductComment _prodComment = _ProductCommentRepo.Get(c=>c.Id == prodCommentVM.Id);
                    if (_prodComment != null && _prodComment.UserId == _UserManager.GetUserId(User) && _prodComment.ProdId == prodId)
                    {
                        _prodComment.Title = prodCommentVM.Title;
                        _prodComment.Description = prodCommentVM.Description;
                        _prodComment.Rating = prodCommentVM.Rating;
                        _ProductCommentRepo.update(_prodComment);
                    }
                }
                _ProductCommentRepo.SaveChanges();
                return Content("");
            }
            return PartialView("_UpsertProdComment", prodCommentVM);
        }

        [Authorize]
        public IActionResult DeleteProdComment(int commentId, int prodId)
        {
            ProductComment prodComment = _ProductCommentRepo.Get(c => c.Id == commentId);
            if(prodComment!=null && (prodComment.UserId == _UserManager.GetUserId(User) || User.IsInRole(Role.Admin)))
            {
                _ProductCommentRepo.remove(prodComment);
                _ProductCommentRepo.SaveChanges();
            }
            return RedirectToAction("ProductDetails", "Product", new {Id=prodId});
        }
    }
}









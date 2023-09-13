using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using Microsoft.AspNetCore.Mvc;

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
            CustomerProductsVM.Products = _ProductRepo.Pagination(1, PageSize);
            CustomerProductsVM.PaginationButtons = (int)Math.Ceiling((double)_ProductRepo.Count()/ (double)PageSize);
            return View(CustomerProductsVM);
        }
        public IActionResult Paginate(int PageNumber)
        {
            return PartialView("~/Views/Shared/Customer_ProductPV/_ProductPV.cshtml",_ProductRepo.Pagination(PageNumber,PageSize));
        }
        
    }
}

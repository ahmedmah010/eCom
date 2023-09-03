using eCom.DataAccess.Data;
using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.DataAccess.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context) 
        {
            _context = context;
            product = new Repo<Product>(_context);
            category = new Repo<Category>(_context);
            tag = new Repo<Tag>(_context);
            productTag = new Repo<ProductTag>(_context);
            productImage = new Repo<ProductImage>(_context);

        }
        public IRepo<Product> product { get;  set; }
        public IRepo<Category> category { get;  set; }
        public IRepo<Tag> tag { get;  set; }
        public IRepo<ProductTag> productTag { get; set; }

        public IRepo<ProductImage> productImage { get; set; }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}

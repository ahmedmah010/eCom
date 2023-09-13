using eCom.DataAccess.Data;
using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCom.DataAccess.Repos
{
    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _db;
        public Repo(AppDbContext db) 
        {
            _db = db;
        }
       
        public void add(T item)
        {
            _db.Set<T>().Add(item);
        }
        public void remove(T item)
        {
            _db.Set<T>().Remove(item);
        }

        public void update(T item)
        {
            _db.Set<T>().Update(item);
        }

        public T Get(Expression<Func<T, bool>> match)
        {
            return _db.Set<T>().FirstOrDefault(match);
        }
        public T Get(Expression<Func<T, bool>> match, params string[] tables)
        {
            IQueryable<T> q = _db.Set<T>();
            foreach(string table in tables)
            {
                q = q.Include(table);    
            }
            return q.FirstOrDefault(match);
        }
        public IEnumerable<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }

        public IEnumerable<T> GetAll(params string[] tables)
        {
            IQueryable<T> q = _db.Set<T>();
            foreach (var table in tables) 
            {
                q = q.Include(table);
                
            }
            return q.ToList();
        }
        public EntityEntry<T> GetEntity(T item)
        {
            return _db.Entry(item);
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        public List<T> Pagination(int page, int pageSize)
        {
            return _db.Set<T>().Skip((page-1)*pageSize).Take(pageSize).ToList();
        }
        public int Count()
        {
            return _db.Set<T>().Count();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyClientForMSExchange.Repositories.Interfaces
{
    using System.Data.Entity;
    using System.Linq.Expressions;

    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> DbSet;

        private DbContext db;

        public Repository(DbContext dataContext)
        {
            db = dataContext;
            DbSet = dataContext.Set<T>();
        }

        #region IRepository<T> Members

        public void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
            
        }

        public void Save()
        {
            db.SaveChanges();
        }

        #endregion
    }
}
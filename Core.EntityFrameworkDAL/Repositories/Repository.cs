namespace Core.EntityFrameworkDAL.Repositories
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    /// <summary>
    /// The repository.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// The db set.
        /// </summary>
        private DbSet<T> DbSet;

        /// <summary>
        /// The db.
        /// </summary>
        private DbContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="dataContext">
        /// The data context.
        /// </param>
        public Repository(DbContext dataContext)
        {
            this.db = dataContext;
            this.DbSet = dataContext.Set<T>();
        }

        #region IRepository<T> Members

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(T entity)
        {
            this.DbSet.Add(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Delete(T entity)
        {
            this.DbSet.Remove(entity);
        }

        /// <summary>
        /// The search for.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<T> GetAll()
        {
            return this.DbSet;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T GetById(int id)
        {
            return this.DbSet.Find(id);
        }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            this.db.SaveChanges();
        }

        #endregion
    }
}
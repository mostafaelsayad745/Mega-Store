using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // This is a generic repository interface that will be implemented by all the repositories in the project
        // This interface will have all the basic CRUD operations that will be implemented by the repositories
        // _context.Cateigories.where(c=>c.Id==1).ToList(); // the part Expression is added to support the where clause in the query
        Task<IEnumerable<T>> GetAll(Expression<Func<T,bool>>? predicate = null , string? includeword = null);
        Task<T> GetFirstOrDefault(Expression<Func<T,bool>>? predicate = null , string? includeword = null);
        Task Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

    }
}

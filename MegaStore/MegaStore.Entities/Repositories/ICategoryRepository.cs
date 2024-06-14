using MegaStore.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.Entities.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // This is a specific repository interface that will be implemented by the CategoryRepository
        // This interface will have all the specific operations that will be implemented by the CategoryRepository
        Task Update(Category entity);
    }
}

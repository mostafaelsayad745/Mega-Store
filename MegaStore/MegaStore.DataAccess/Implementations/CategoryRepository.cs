using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MegaStore.DataAccess.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly MegaStoreDbContext _context;

        public CategoryRepository(MegaStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Category entity)
        {
            var CategoryInDb = await _context.Categories.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (CategoryInDb != null)
            {
                CategoryInDb.Name = entity.Name;
                CategoryInDb.Description = entity.Description;
                CategoryInDb.CreatedTime = DateTime.Now;
            }
        }
    }
}

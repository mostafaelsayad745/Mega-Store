using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MegaStore.DataAccess.Implementations
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly MegaStoreDbContext _context;

        public ApplicationUserRepository(MegaStoreDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

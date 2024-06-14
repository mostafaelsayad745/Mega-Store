using MegaStore.DataAccess.Data;
using MegaStore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.DataAccess.Implementations
{
    // This class is used to implement the IUnitOfWork interface
    public class UnitOfWork : IUnitOfWork
    {

        private readonly MegaStoreDbContext _context;

        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IShoppingCartRepository ShoppingCarts { get; private set; }
        public IOrderHeaderRepository OrderHeaders { get; private set; }

		public IOrderDetailRepository OrderDetails { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
		public UnitOfWork(MegaStoreDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            ShoppingCarts = new ShoppingCartRepository(_context);
            OrderHeaders = new OrderHeaderRepository(_context);
            OrderDetails = new OrderDetailRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

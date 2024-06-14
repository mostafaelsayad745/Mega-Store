using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MegaStore.DataAccess.Implementations
{
    // This class is used to implement the IShoppingCartRepository interface
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly MegaStoreDbContext _context;

        public ShoppingCartRepository(MegaStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public int DecreaseCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncreaseCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}

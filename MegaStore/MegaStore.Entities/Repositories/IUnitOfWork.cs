using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.Entities.Repositories
{
    // This interface is used to define the contract for the UnitOfWork class
    public interface IUnitOfWork : IDisposable
    {
        // This is a unit of work interface that will be implemented by the UnitOfWork class
        // This interface will have all the repositories that will be used in the project
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IOrderHeaderRepository OrderHeaders { get; }
        IOrderDetailRepository OrderDetails { get; }
        IApplicationUserRepository ApplicationUser { get; }
        Task<int> Complete();
    }
}

using MegaStore.Entities.Models;


namespace MegaStore.Entities.Repositories
{
    // This interface is used to define the contract for the ShoppingCart Repository
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        // Define the methods that will be implemented in the ShoppingCartRepository class
        // These methods will be used to increase or decrease the count of a product in the shopping cart
       int IncreaseCount(ShoppingCart shoppingCart, int count);
       int DecreaseCount(ShoppingCart shoppingCart, int count);
    }
}

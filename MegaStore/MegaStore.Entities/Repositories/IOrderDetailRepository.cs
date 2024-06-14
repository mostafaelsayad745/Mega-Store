using MegaStore.Entities.Models;

namespace MegaStore.Entities.Repositories
{
	public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
	{
		// This is a specific repository interface that will be implemented by the OrderDetailRepository
		// This interface will have all the specific operations that will be implemented by the OrderDetailRepository
		Task Update(OrderDetail orderDetail);
	}
}

using MegaStore.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.Entities.Repositories
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
		// This is a specific repository interface that will be implemented by the OrderHeaderRepository
		// This interface will have all the specific operations that will be implemented by the OrderHeaderRepository
		Task Update(OrderHeader orderHeader);

        Task UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus);
    }
}

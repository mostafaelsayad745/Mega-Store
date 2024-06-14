using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MegaStore.DataAccess.Implementations
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly MegaStoreDbContext _context;

        public OrderHeaderRepository(MegaStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

		public async Task UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
		{
			var orderfromDB = await _context.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);
            if (orderfromDB != null)
            { 
                orderfromDB.OrderStatus = OrderStatus;
                orderfromDB.PaymentDate = DateTime.Now;
                if (PaymentStatus != null)
                {
                    orderfromDB.PaymentStatus = PaymentStatus;
                }
            }

		}
	}
}

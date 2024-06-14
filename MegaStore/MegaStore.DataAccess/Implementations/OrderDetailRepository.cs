using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.DataAccess.Implementations
{
	public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
	{
		private readonly MegaStoreDbContext _context;

		public OrderDetailRepository(MegaStoreDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task Update(OrderDetail orderDetail)
		{
			_context.OrderDetails.Update(orderDetail);
		}

		
	}
	
	
}

using MegaStore.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.Entities.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task Update(Product product);
    }
}

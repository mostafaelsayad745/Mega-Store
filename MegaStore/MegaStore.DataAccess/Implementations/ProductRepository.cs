using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.DataAccess.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly MegaStoreDbContext _context;

        public ProductRepository(MegaStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Product product)
        {
            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (productInDb != null)
            {
                productInDb.Price = product.Price;
                productInDb.Description = product.Description;
                productInDb.Img = product.Img;
                productInDb.Name = product.Name;
                productInDb.CategoryId = product.CategoryId;
            }
        }
    }
}

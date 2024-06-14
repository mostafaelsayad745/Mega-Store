using MegaStore.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MegaStore.DataAccess.Data
{
    public class MegaStoreDbContext : IdentityDbContext<IdentityUser>
    {
        public MegaStoreDbContext(DbContextOptions<MegaStoreDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public DbSet<OrderHeader> OrderHeaders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

    }
}

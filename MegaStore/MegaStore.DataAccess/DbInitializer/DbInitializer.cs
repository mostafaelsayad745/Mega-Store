

using MegaStore.DataAccess.Data;
using MegaStore.Entities.Models;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace MegaStore.DataAccess.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly MegaStoreDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public DbInitializer(
			MegaStoreDbContext context,
			UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager
			)
        {
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}
        public void Initialize()
		{
			// migration
			try
			{
				if (_context.Database.GetPendingMigrations().Count() > 0)
				{
					_context.Database.Migrate();
				}
			}
			catch (Exception)
			{

				throw;
			}

			//roles

			if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
			{
				 _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();

				//user

				_userManager.CreateAsync(new ApplicationUser
				{
					UserName = "Admin",
					Email = "Admin@MegaStore.com",
					Name = "Administrator",
					PhoneNumber = "1234567890",
					Address = "alex",
					City = "alex"
				},"Admin123").GetAwaiter().GetResult();
				ApplicationUser user = _context.Users.FirstOrDefault(u => u.Email == "Admin@MegaStore.com");

				_userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();
			}


			return;
		}
		
	}
}


using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;

namespace MegaStore.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task< IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;

            var products = await _unitOfWork.Products.GetAll();
            var productsPaged = products.ToPagedList(pageNumber, pageSize);
            return View(productsPaged);
        }

        public async Task<IActionResult> Details(int id)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = await _unitOfWork.Products.GetFirstOrDefault(v => v.Id == id, includeword: "Category"),
                Count = 1
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var shoppingCartFromDb = await _unitOfWork.ShoppingCarts.GetFirstOrDefault(
             v => v.ApplicationUserId == shoppingCart.ApplicationUserId && v.ProductId == shoppingCart.Product.Id);

            if (shoppingCartFromDb == null)
            {
                // Create a new ShoppingCart object 
                var newShoppingCart = new ShoppingCart
                {
                    ProductId = shoppingCart.Product.Id,
                    Count = shoppingCart.Count,
                    ApplicationUserId = shoppingCart.ApplicationUserId
                };
                await _unitOfWork.ShoppingCarts.Add(newShoppingCart);
                await _unitOfWork.Complete();
                var count = await _unitOfWork.ShoppingCarts.GetAll(v => v.ApplicationUserId == claim.Value);
                HttpContext.Session.SetInt32(SD.SessionKey, count.ToList().Count());
				
			}
            else
            {
                 _unitOfWork.ShoppingCarts.IncreaseCount(shoppingCartFromDb, shoppingCart.Count);
				await _unitOfWork.Complete();
			}
            
            

            return RedirectToAction(nameof(Index));
        }

    }
}

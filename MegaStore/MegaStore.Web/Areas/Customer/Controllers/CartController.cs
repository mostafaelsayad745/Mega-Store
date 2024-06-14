using MegaStore.DataAccess.Implementations;
using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using MegaStore.Entities.ViewModels;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace MegaStore.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				CartsList = await _unitOfWork.ShoppingCarts.GetAll(v => v.ApplicationUserId == claim.Value, includeword: "Product"),
				OrderHeader = new()
            };
            foreach (var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.TotalCarts += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }


        public async Task<IActionResult> Plus(int cartid)
        {
            var shoppingCart =await _unitOfWork.ShoppingCarts.GetFirstOrDefault(x => x.Id == cartid);
            if (shoppingCart != null)
            {
                _unitOfWork.ShoppingCarts.IncreaseCount(shoppingCart, 1);
				await _unitOfWork.Complete();
			}
            return RedirectToAction("Index");
        }

		public async Task<IActionResult> Minus(int cartid)
		{
			var shoppingCart = await _unitOfWork.ShoppingCarts.GetFirstOrDefault(x => x.Id == cartid);
            if (shoppingCart.Count <= 1)
            {
                 _unitOfWork.ShoppingCarts.Remove(shoppingCart);
                var count = await _unitOfWork.ShoppingCarts.GetAll(v => v.ApplicationUserId == shoppingCart.ApplicationUserId);
                HttpContext.Session.SetInt32(SD.SessionKey, count.ToList().Count()-1);
            }
			else
            {
				_unitOfWork.ShoppingCarts.DecreaseCount(shoppingCart, 1);
				
			}
			await _unitOfWork.Complete();
			return RedirectToAction("Index");
		}

		
		public async Task<IActionResult> Remove(int cartId)
		{
			var cart = await _unitOfWork.ShoppingCarts.GetFirstOrDefault(v => v.Id == cartId);
			if (cart != null)
			{
				_unitOfWork.ShoppingCarts.Remove(cart);
				await _unitOfWork.Complete();
                var count = await _unitOfWork.ShoppingCarts.GetAll(v => v.ApplicationUserId == cart.ApplicationUserId);
                HttpContext.Session.SetInt32(SD.SessionKey, count.ToList().Count());
            }
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				CartsList = await _unitOfWork.ShoppingCarts.GetAll(v => v.ApplicationUserId == claim.Value, includeword: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;


			foreach (var item in ShoppingCartVM.CartsList)
			{
				ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
			}
			return View(ShoppingCartVM);

			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Summary")]
		public async Task<IActionResult> PostSummary(ShoppingCartVM shoppingCartVM)
		{
			var calimsIdentity = (ClaimsIdentity) User.Identity;
			var claim = calimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			
			shoppingCartVM.CartsList = await _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == claim.Value, includeword: "Product");

			shoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
			shoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
			shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in shoppingCartVM.CartsList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }

			// to save order header data in the order header table in the database but you still not complete the payment process

			await _unitOfWork.OrderHeaders.Add(shoppingCartVM.OrderHeader);
			await _unitOfWork.Complete();
            // to save order details data in the order details table in the database but you still not complete the payment process
            foreach (var item in shoppingCartVM.CartsList)
			{
				OrderDetail orderDetail = new OrderDetail()
				{
					ProductId = item.ProductId,
					OrderHeaderId = shoppingCartVM.OrderHeader.Id,
					Price = item.Product.Price,
					Count = item.Count
				};

				await _unitOfWork.OrderDetails.Add(orderDetail);
				await _unitOfWork.Complete();
			}

			var domain = "https://localhost:7084/";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
       
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/orderconfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/index",
            };

			foreach (var item in shoppingCartVM.CartsList)
			{

				var sessionlineoption = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price*100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionlineoption);
			}
			// this is used to get the session id to save it in the order header in the database this is means the Consent that the order is done 
            var service = new SessionService();
            Session session = service.Create(options);
			shoppingCartVM.OrderHeader.SessionId = session.Id;
			
			await _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            
        }

		public async Task<IActionResult> OrderConfirmation(int id)
		{
			OrderHeader orderHeader = await _unitOfWork.OrderHeaders.GetFirstOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            
            if (session.PaymentStatus.ToLower() == "paid")
			{
			    await _unitOfWork.OrderHeaders.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntentId = session.PaymentIntentId;

                await _unitOfWork.Complete();
			}
			var shoppingCarts = await _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId);
            _unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);
            await _unitOfWork.Complete();

            return View(id);
        }

    }
}

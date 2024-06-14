using MegaStore.Entities.Repositories;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MegaStore.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public async Task< IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                else
                {
                    var count = await _unitOfWork.ShoppingCarts.GetAll(c => c.ApplicationUserId == claim.Value);
                    var countedItmes = count.ToList().Count();

                    HttpContext.Session.SetInt32(SD.SessionKey, countedItmes);
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
           
        }
    }
}

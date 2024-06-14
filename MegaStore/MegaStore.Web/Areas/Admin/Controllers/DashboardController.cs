using MegaStore.Entities.Repositories;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MegaStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task< IActionResult> Index()
        {
            ViewBag.Orders =  _unitOfWork.OrderHeaders.GetAll().Result.Count();
            ViewBag.ApprobedOrders = _unitOfWork.OrderHeaders.GetAll(x => x.OrderStatus == SD.Approve).Result.Count();
            ViewBag.Users = _unitOfWork.ApplicationUser.GetAll().Result.Count();
            ViewBag.Products =  _unitOfWork.Products.GetAll().Result.Count();
            return View();
        }
    }
}

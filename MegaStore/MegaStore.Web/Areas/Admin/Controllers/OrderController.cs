using MegaStore.Entities.Models;
using MegaStore.Entities.Repositories;
using MegaStore.Entities.ViewModels;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;


namespace MegaStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM VM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var orderHeaders = await _unitOfWork.OrderHeaders.GetAll(includeword: "ApplicationUser");
            return View(orderHeaders);
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = await _unitOfWork.OrderHeaders.GetAll(includeword : "ApplicationUser");
            return Json(new {data = orderHeaders});
        }

        public async Task<IActionResult> Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                orderHeader = await _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == orderid, includeword: "ApplicationUser"),
                orderDetails = await _unitOfWork.OrderDetails.GetAll(u => u.OrderHeaderId == orderid, includeword: "Product"),
            };
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderFromDb = await _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == VM.orderHeader.Id);
            orderFromDb.Name = VM.orderHeader.Name;
            orderFromDb.Phone = VM.orderHeader.Phone;
            orderFromDb.Address = VM.orderHeader.Address;
            orderFromDb.City = VM.orderHeader.City;
            

            if (VM.orderHeader.CarrierI != null)
            {
                orderFromDb.CarrierI = VM.orderHeader.CarrierI;
            }

            if (VM.orderHeader.TrackingNumber != null)
            {
                orderFromDb.TrackingNumber = VM.orderHeader.TrackingNumber;
            }

            await _unitOfWork.OrderHeaders.Update(orderFromDb);

            await _unitOfWork.Complete();

            TempData["Update"] = "Ithem has updated successfully";
            return RedirectToAction("Details","Order" , new { orderid = orderFromDb.Id});

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProcess()
        {
            await _unitOfWork.OrderHeaders.UpdateOrderStatus(VM.orderHeader.Id, SD.Processing, null);

            await _unitOfWork.Complete();

            TempData["Update"] = "Order Status has updated successfully";
            return RedirectToAction("Details", "Order", new { orderid = VM.orderHeader.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShip()
        {
            var orderFromDb = await _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == VM.orderHeader.Id);
            orderFromDb.TrackingNumber = VM.orderHeader.TrackingNumber;
            orderFromDb.CarrierI = VM.orderHeader.CarrierI;
            orderFromDb.OrderStatus = VM.orderHeader.OrderStatus;
            orderFromDb.ShippingDate = DateTime.Now;

            await _unitOfWork.OrderHeaders.Update(orderFromDb);

            await _unitOfWork.Complete();

            TempData["Update"] = "Order has shipped successfully";
            return RedirectToAction("Details", "Order", new { orderid = VM.orderHeader.Id });

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var orderFromDb = await _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == VM.orderHeader.Id);
           
            if(orderFromDb.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                
                await _unitOfWork.OrderHeaders.UpdateOrderStatus(orderFromDb.Id,SD.Cancelled,SD.Refund);
             }
            else
            {
                await _unitOfWork.OrderHeaders.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Refund);
            }

          
            await _unitOfWork.Complete();

            TempData["Update"] = "Order Status has Updated successfully";
            return RedirectToAction("Details", "Order", new { orderid = VM.orderHeader.Id });

        }
    }
}

using MegaStore.Entities.Models;
using MegaStore.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using MegaStore.Entities.Repositories;
using MegaStore.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MegaStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAll();
            return View(categories);
        }

        [HttpGet]
        public  IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // This is to prevent Cross-Site Request Forgery (CSRF) attacks : means that the form data is coming from the same site
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();
            }
            var category = await _unitOfWork.Categories.GetFirstOrDefault(e => e.Id == id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Categories.Update(category);
                await _unitOfWork.Complete();
                TempData["Edit"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();
            }
            var category = await _unitOfWork.Categories.GetFirstOrDefault(e => e.Id == id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            var category = await _unitOfWork.Categories.GetFirstOrDefault(e => e.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Categories.Remove(category);
            TempData["Delete"] = "Category Deleted Successfully";
            await _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}

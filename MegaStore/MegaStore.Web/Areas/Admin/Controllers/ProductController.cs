using MegaStore.Entities.Models;
using MegaStore.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using MegaStore.Entities.Repositories;
using MegaStore.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MegaStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAll(includeword: "Category");
            return View(products);
        }

        public async Task<IActionResult> GetData()
        {
            var products = await _unitOfWork.Products.GetAll(includeword: "Category");
            return Json(new { data = products });
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Categories.GetAll() ?? new List<Category>();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList()
            };
            return View(productVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // This is to prevent Cross-Site Request Forgery (CSRF) attacks : means that the form data is coming from the same site
        public IActionResult Create(ProductVM productVM , IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(RootPath, @"Images\Products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + extension;

                }


                _unitOfWork.Products.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();
            }
            var categories = await _unitOfWork.Categories.GetAll() ?? new List<Category>();
            ProductVM productVM = new ProductVM()
            {
                Product = await _unitOfWork.Products.GetFirstOrDefault(e => e.Id == id),
                CategoryList = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList()
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM productVM , IFormFile? file )
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(RootPath, @"Images\Products");
                    var extension = Path.GetExtension(file.FileName);

                    if (productVM.Product.Img != null)
                    {
                        var oldImage = Path.Combine(RootPath, productVM.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + extension;

                }

                await _unitOfWork.Products.Update(productVM.Product);
                await _unitOfWork.Complete();
                TempData["Edit"] = "Product Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            var product = await _unitOfWork.Products.GetFirstOrDefault(e => e.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _unitOfWork.Products.Remove(product);
            var RootPath = _webHostEnvironment.WebRootPath;
            var oldImage = Path.Combine(RootPath, product.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            TempData["Delete"] = "Product Deleted Successfully";
            await _unitOfWork.Complete();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}

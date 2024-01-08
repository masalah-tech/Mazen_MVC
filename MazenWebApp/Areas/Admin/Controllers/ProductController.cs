using Mazen.DataAccess.Repository.IRepository;
using Mazen.Utility;
using MazenWebApp.Models;
using MazenWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace MazenWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, 
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products =
                _unitOfWork.ProductRepository
                .GetAll(includePropeties: "Category");

            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> categoryList =
            //    _unitOfWork.CategoryRepository
            //    .GetAll().Select(c => new SelectListItem
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString()
            //    });

            //ViewBag.CategoryList = categoryList;

            //ViewData["CategoryList"] = categoryList;

            var productVM = new ProductVM
            {
                CategoryList = 
                    _unitOfWork.CategoryRepository
                    .GetAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                Product = new Product()
            };

            if (id != null && id != 0)
            {
                // update
                productVM.Product = 
                    _unitOfWork.ProductRepository
                    .Get(p => p.Id == id);
            }

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? productImg)
        {
            if (ModelState.IsValid)
            {
                string wwwwRootPath = _webHostEnvironment.WebRootPath;

                if (productImg != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productImg.FileName);
                    string productPath = Path.Combine(wwwwRootPath, @"images/product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // Delete the old image
                        var oldImagePath = 
                            Path.Combine(wwwwRootPath, productVM.Product.ImageUrl.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        productImg.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"/images/product/" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList =
                    _unitOfWork.CategoryRepository
                    .GetAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });
            }

            return View(productVM);
        }

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int id)
        //{
        //    var product =
        //        _unitOfWork
        //        .ProductRepository
        //        .Get(p => p.Id == id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.ProductRepository.Remove(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product deleted successfully";
        //        return RedirectToAction("Index");
        //    }

        //    return View();
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() 
        {
            var products =
                _unitOfWork.ProductRepository
                .GetAll(includePropeties: "Category");

            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id) 
        {
            var product =
                _unitOfWork.ProductRepository
                .Get(p => p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var oldImagePath =
                Path.Combine(_webHostEnvironment.WebRootPath, 
                    product.ImageUrl.TrimStart('/'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product deleted successfully" });
        }
        #endregion
    }
}

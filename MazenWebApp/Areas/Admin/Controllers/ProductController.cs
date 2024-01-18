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
                    .Get(p => p.Id == id, includePropeties: "ProductImages");
            }

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? productImgs)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }

                _unitOfWork.Save();

                string wwwwRootPath = _webHostEnvironment.WebRootPath;

                if (productImgs != null)
                {
                    foreach (var img in productImgs)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        string productPath = @"images/products/product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            img.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = "/" + productPath + "/" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    _unitOfWork.ProductRepository.Update(productVM.Product);
                    _unitOfWork.Save();
                }

                TempData["success"] = "Product created/updated successfully";
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

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int id)
        {
            var product =
                _unitOfWork
                .ProductRepository
                .Get(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Remove(product);
                _unitOfWork.Save();
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imgToBeDeleted = 
                _unitOfWork.ProductImageRepository
                .Get(pi => pi.Id == imageId);

            if (imgToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imgToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                        Path.Combine(_webHostEnvironment.WebRootPath,
                            imgToBeDeleted.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.ProductImageRepository.Remove(imgToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = imgToBeDeleted.ProductId });
        }

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

            //var oldImagePath =
            //    Path.Combine(_webHostEnvironment.WebRootPath, 
            //        product.ImageUrl.TrimStart('/'));

            //if (System.IO.File.Exists(oldImagePath))
            //{
            //    System.IO.File.Delete(oldImagePath);
            //}

            string productPath = @"images/products/product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);

                foreach (var path in filePaths)
                {
                    System.IO.File.Delete(path);
                }

                Directory.Delete(finalPath);
            }


            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product deleted successfully" });
        }
        #endregion
    }
}

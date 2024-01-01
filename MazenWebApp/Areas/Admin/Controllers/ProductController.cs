﻿using Mazen.DataAccess.Repository.IRepository;
using MazenWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MazenWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products =
                _unitOfWork.ProductRepository
                .GetAll();

            return View(products);
        }

        public IActionResult Edit(int id)
        {
            var product = 
                _unitOfWork.ProductRepository
                .Get(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            //int id = product.Id;
            //var products = 
            //    _unitOfWork.ProductRepository
            //    .GetAll();

            //if (_unitOfWork.ProductRepository
            //    .GetAll()
            //    .Any(p => p.Id != product.Id && p.ISBN == product.ISBN)) 
            //{
            //    ModelState.AddModelError("ISBN", "Book already exists");
            //}
            
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }

            //foreach (var productItem in _unitOfWork.ProductRepository.GetAll())
            //{
            //    if (productItem.Id != product.Id
            //        && productItem.ISBN == product.ISBN)
            //    {
            //        ModelState.AddModelError("ISBN", "Book already exists");
            //    }
            //}

            

            return View();
        }
    }
}
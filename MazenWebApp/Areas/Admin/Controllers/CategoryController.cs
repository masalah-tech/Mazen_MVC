using Mazen.DataAccess.Repository;
using Mazen.DataAccess.Repository.IRepository;
using MazenWebApp.DataAccess.Data;
using MazenWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MazenWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var objCategoryList =
                _unitOfWork.CategoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name and Display Order must not be the Same");
            }
            if (category.Name != null && category.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "test is an invalid value");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                //return RedirectToAction("Index", "Category");
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id < 1)
            {
                return NotFound();
            }

            var category =
                _unitOfWork.CategoryRepository
                .Get(c => c.Id == id);

            //var category =
            //    _context.Categories
            //    .FirstOrDefault(c => c.Id == id);

            //var category =
            //    _context.Categories
            //    .Where(c => c.Id == id)
            //    .FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name and Display Order must not be the Same");
            }
            if (category.Name != null && category.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "test is an invalid value");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                //return RedirectToAction("Index", "Category");
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id < 1)
            {
                return NotFound();
            }

            var category =
                _unitOfWork.CategoryRepository
                .Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var category =
                _unitOfWork.CategoryRepository
                .Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index");
        }
    }
}

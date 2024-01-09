using Mazen.DataAccess.Repository.IRepository;
using Mazen.Utility;
using MazenWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MazenWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var companies = 
                _unitOfWork.CompanyRepository.GetAll();

            return View(companies);
        }

        public IActionResult Upsert(int? id) 
        {
            var company = 
                _unitOfWork.CompanyRepository
                .Get(c => c.Id == id);

            if (company != null) 
            { 
                return View(company);
            }


            return View(new Company());
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(company);
                    _unitOfWork.Save();
                    TempData["success"] = "Company added successfully";
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(company);
                    _unitOfWork.Save();
                    TempData["success"] = "Company updated successfully";
                }

                return RedirectToAction("Index");
            }

            return View(company);
        }

        

        #region API CAlls

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies =
                _unitOfWork.CompanyRepository.GetAll();

            return Json(new { data = companies });
        }

        public IActionResult Delete(int id)
        {
            var company =
                _unitOfWork.CompanyRepository.Get(c => c.Id == id);

            if (company == null)
            {
                return Json(new { success = false, message = "Company not found" });
            }

            _unitOfWork.CompanyRepository.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleted successfully" });
        }

        #endregion
    }
}

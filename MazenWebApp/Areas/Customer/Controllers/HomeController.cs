using Mazen.DataAccess.Repository.IRepository;
using MazenWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MazenWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products =
                _unitOfWork.ProductRepository
                .GetAll(includePropeties: "Category");

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product =
                _unitOfWork.ProductRepository
                .Get(p => p.Id == id, includePropeties: "Category");

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
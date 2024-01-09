using Mazen.DataAccess.Repository.IRepository;
using MazenWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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

        public IActionResult Details(int productId)
        {
            var cart = new ShoppingCart
            {
                Product = _unitOfWork.ProductRepository
                    .Get(p => p.Id == productId, includePropeties: "Category"),
                Count = 1,
                ProductId = productId,

            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb =
                _unitOfWork.ShoppingCartRepository
                .Get(sc => sc.ApplicationUserId == userId
                    && sc.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                // shopping cart already exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            }
            else
            {
                // add new cart
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
            }

            _unitOfWork.Save();
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
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
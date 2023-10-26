using Microsoft.AspNetCore.Mvc;

namespace MazenWebApp.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

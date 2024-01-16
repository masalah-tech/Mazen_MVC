using Mazen.DataAccess.Repository.IRepository;
using Mazen.Utility;
using MazenWebApp.DataAccess.Data;
using MazenWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MazenWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        #region API CAlls

        [HttpGet]
        public IActionResult GetAll()
        {
            var users =
                _context.ApplicationUsers.
                Include(u => u.Company)
                .ToList();

            var userRoles =
                _context.UserRoles
                .ToList();

            var roles =
                _context.Roles
                .ToList();

            foreach (var user in users)
            {
                var roleId =
                    userRoles
                    .FirstOrDefault(u => u.UserId == user.Id)
                    .RoleId;
                user.Role =
                    roles.FirstOrDefault(r => r.Id == roleId)
                    .Name;

                if (user.Company == null)
                {
                    user.Company = new Company
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var user =
                _context.ApplicationUsers
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // User is currently locked, we will unlock him
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _context.SaveChanges();

            return Json(new { success = true, message = "User status changed successfully" });
        }

        #endregion
    }
}

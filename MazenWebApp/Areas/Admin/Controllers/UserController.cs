using Mazen.DataAccess.Repository.IRepository;
using Mazen.Utility;
using MazenWebApp.DataAccess.Data;
using MazenWebApp.Models;
using MazenWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Radar;
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

        public IActionResult RoleManagement(string userId)
        {
            var user = 
                _context.ApplicationUsers
                .Include(u => u.Company)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var userRoleMapper =
                _context.UserRoles
                .FirstOrDefault(ur => ur.UserId == userId);

            if (userRoleMapper == null)
            {
                return NotFound();
            }

            var userRole =
                _context.Roles
                .FirstOrDefault(r => r.Id == userRoleMapper.RoleId);

            if (userRole == null)
            {
                return NotFound();
            }

            var companies =
                _context.Companies
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            var roles =
                _context.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                });


            RoleManagementVM roleManagementVM = new RoleManagementVM
            {
                UserId = user.Id,
                UserName = user.Name,
                Companies = companies,
                Roles = roles,
                RoleId = userRole.Id,
                CompanyId = user.CompanyId,
                UserRole = userRole.Name
            };

            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            var user =
                _context.ApplicationUsers
                .FirstOrDefault(u => u.Id == roleManagementVM.UserId);

            user.Name = roleManagementVM.UserName;

            if (ModelState.IsValid)
            {
                var userRoleFromDb =
                    _context.UserRoles
                    .FirstOrDefault(u => u.UserId == roleManagementVM.UserId);

                _context.UserRoles.Remove(userRoleFromDb);
                _context.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = roleManagementVM.RoleId,
                    UserId = roleManagementVM.UserId,
                });

                var newRole =
                    _context.Roles
                    .FirstOrDefault(r => r.Id == roleManagementVM.RoleId)
                    .Name;

                if (newRole == SD.Role_Company 
                    && roleManagementVM.CompanyId != null)
                {
                    user.CompanyId = roleManagementVM.CompanyId;
                }
                else
                {
                    user.Company = null;
                    user.CompanyId = null;
                }

                _context.ApplicationUsers.Update(user);
                _context.SaveChanges();

                TempData["success"] = "User permissions updated successfully";

                return RedirectToAction("Index");
            }

            roleManagementVM.Roles =
                _context.Roles
                .Select(r => new SelectListItem {
                    Text = r.Name,
                    Value = r.Id
                });

            roleManagementVM.Companies =
                _context.Companies
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            var userRoleMapper =
                _context.UserRoles
                .FirstOrDefault(ur => ur.UserId == roleManagementVM.UserId);

            if (userRoleMapper == null)
            {
                return NotFound();
            }

            var userRole =
                _context.Roles
                .FirstOrDefault(r => r.Id == userRoleMapper.RoleId);

            if (userRole == null)
            {
                return NotFound();
            }

            roleManagementVM.UserRole = userRole.Name;
            roleManagementVM.CompanyId = user.CompanyId;

            return View(roleManagementVM);
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

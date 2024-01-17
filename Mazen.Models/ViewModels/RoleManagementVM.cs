using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazenWebApp.Models.ViewModels
{
    public class RoleManagementVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Companies { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Roles { get; set; }
        public string RoleId { get; set; }
        public int? CompanyId { get; set; }
        [ValidateNever]
        public string UserRole { get; set; }
    }
}

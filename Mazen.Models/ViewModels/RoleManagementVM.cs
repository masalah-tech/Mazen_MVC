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
        public ApplicationUser User { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}

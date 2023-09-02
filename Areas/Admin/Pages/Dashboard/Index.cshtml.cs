#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;

        }

        public string ReturnUrl { get; set; }
        public int NumberOfUsers { get; set; }
        public int NumberOfRoles { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                NumberOfUsers = _userManager.Users.Where(user => !user.UserName.Contains("SuperAdmin")).Count();
                NumberOfRoles = _roleManager.Roles.Where(role => !role.Name.Contains("SuperAdmin")).Count();
            }
        }
    }
}

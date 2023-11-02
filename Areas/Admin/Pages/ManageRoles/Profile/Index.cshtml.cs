#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.ManageRoles.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationRole RoleData { get; set; }
        public byte[] RoleImage { get; set; }

        public string ReturnUrl { get; set; }

        public void LoadAsync(ApplicationRole role)
        {
            if (ModelState.IsValid)
            {
                RoleData = role;
                RoleImage = role.Icon;
            }
        }

        public async Task OnGet(string id, string returnUrl)
        {
            returnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                ApplicationRole role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    LoadAsync(role);
                }
            }
        }
    }
}

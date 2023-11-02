#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Admin.Pages.ManageRoles.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class UsersModel : PageModel
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<ApplicationRole> _roleManager;
        public readonly ApplicationDbContext _db;

        public UsersModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public string ReturnUrl { get; set; }
        public int NoOfUser { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public IList<ApplicationUserRole> RoleUsers { get; set; }
        public ApplicationRole RoleData { get; set; }
        public byte[] RoleImage { get; set; }

        public async Task LoadAsync(ApplicationRole role)
        {
            if (ModelState.IsValid)
            {
                RoleData = role;
                RoleImage = role.Icon;
                RoleUsers = await _db.UserRoles.Where(r => r.RoleId == role.Id).Include(r => r.ApplicationUser).ToListAsync();
            }
        }

        public async Task OnGet(string id, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                RoleUsers = await _db.UserRoles.Where(r => r.RoleId == role.Id).Include(r => r.ApplicationUser).ToListAsync();
                await LoadAsync(role);
            }
        }
    }
}

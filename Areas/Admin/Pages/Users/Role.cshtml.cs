#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AdminPanel.Areas.Admin.Pages.User
{
    [Authorize(Roles = "SuperAdmin")]
    public class RoleModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
        }

        public string ReturnUrl { get; set; }
        public IList<SelectListItem> RoleList { get; set; }
        public ApplicationUser UserData { get; set; }

        public async Task OnGet(string id, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    UserData = user;
                    RoleList = await _roleManager.Roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(List<string> RoleList, string id, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    UserData = user;
                    if (RoleList.Count > 0)
                    {
                        foreach (var roleList in RoleList)
                        {
                            var role = await _roleManager.FindByIdAsync(roleList);
                            await _userManager.AddToRoleAsync(user, role.ToString());
                        }
                        return RedirectToPage("/Users/Index", new { area = "Admin" });
                    }
                    return Page();
                }
            }
            return Page();
        }
    }
}

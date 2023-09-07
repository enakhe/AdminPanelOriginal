#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.User
{
    [Authorize(Roles = "SuperAdmin")]
    public class AssignRoles : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly ApplicationDbContext _db;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AssignRoles(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
        }

        public string ReturnUrl { get; set; }
        public IList<ManageUserRolesViewModel> RoleList { get; set; }
        public ApplicationUser PersonalInfo { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public async Task OnGet(string id, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {

                    PersonalInfo = user;
                    var model = new List<ManageUserRolesViewModel>();
                    foreach (var role in _roleManager.Roles)
                    {
                        if (!role.Name.Contains("SuperAdmin"))
                        {
                            var userRolesViewModel = new ManageUserRolesViewModel
                            {
                                RoleId = role.Id,
                                RoleName = role.Name
                            };
                            if (await _userManager.IsInRoleAsync(user, role.Name))
                            {
                                userRolesViewModel.Selected = true;
                            }
                            else
                            {
                                userRolesViewModel.Selected = false;
                            }
                            model.Add(userRolesViewModel);
                        }

                    }
                    RoleList = model;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> RoleList, string id, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var result = await _userManager.RemoveFromRolesAsync(user, roles);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot remove user existing roles");
                        return Page();
                    }

                    result = await _userManager.AddToRolesAsync(user, RoleList.Where(x => x.Selected).Select(y => y.RoleName));

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot add selected roles to user");
                        return Page();
                    }

                    if (result.Succeeded)
                    {
                        await _userManager.UpdateAsync(user);
                        _db.SaveChanges();
                        StatusMessage = "Successfully assigned role";
                        return RedirectToPage("/Users/Index", new { area = "Admin", statusMessage = StatusMessage });
                    }
                }
            }
            return Page();
        }
    }
}

#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.ManageUsers.Profile
{


    [Authorize(Roles = "SuperAdmin")]
    public class RolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RolesModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public string ReturnUrl { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationUser UserData { get; set; }
        public UserBackUpInfo UserBackUpInfo { get; set; }
        public IList<ManageUserRolesViewModel> RoleList { get; set; }
        public byte[] UserProfilePicture { get; set; }

        public async Task LoadAsync(ApplicationUser user)
        {
            UserData = user;
            UserProfilePicture = user.ProfilePicture;

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

        public async Task OnGetAsync(string id, string returnUrl = null)
        {
            returnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await LoadAsync(user);
                }
            }
            else
            {
                StatusMessage = "Error, something unexpected happened";
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
                        await LoadAsync(user);
                        return Page();
                    }
                }
            }
            return Page();
        }
    }
}

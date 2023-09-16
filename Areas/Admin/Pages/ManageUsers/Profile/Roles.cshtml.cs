#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        public IList<RoleViewModel> RoleList { get; set; }
        public IList<ManageUserRolesViewModel> AssignRoleList { get; set; }
        public UserBackUpInfo UserBackUpInfo { get; set; }
        public byte[] UserProfilePicture { get; set; }

        public async Task LoadAsync(ApplicationUser user)
        {
            UserData = user;
            UserProfilePicture = user.ProfilePicture;


            // Loading all the roles a user belongs to
            var userRoles = await _db.UserRoles.Where(userRole => userRole.UserId == user.Id).Include(userRole => userRole.ApplicationRole).ToListAsync();
            var roleModel = new List<RoleViewModel>();
            if (userRoles != null)
            {
                foreach (var userRole in userRoles)
                {
                    var role = await _db.Roles.FirstOrDefaultAsync(ur => ur.Id == userRole.RoleId);

                    var roleViewModel = new RoleViewModel();
                    roleViewModel.RoleName = role.Name;
                    roleViewModel.StartDate = userRole.StartDate;
                    roleViewModel.EndDate = userRole.EndDate;

                    if (userRole.StartDate > DateTime.Now || userRole.EndDate < DateTime.Now)
                    {
                        roleViewModel.isActive = false;
                    }
                    else
                    {
                        roleViewModel.isActive = true;
                        userRole.isActive = true;
                        _db.Entry(userRole).State = EntityState.Modified;

                        roleViewModel.DaysLeft = userRole.EndDate - DateTime.Now;
                    }
                    roleModel.Add(roleViewModel);
                }
                RoleList = roleModel;
            }



            // Loading all roles 
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                if (!role.Name.Contains("SuperAdmin"))
                {
                    var userRole = await _db.UserRoles.FirstOrDefaultAsync(userRole => userRole.UserId == user.Id);

                    var userRolesViewModel = new ManageUserRolesViewModel();

                    userRolesViewModel.RoleId = role.Id;
                    userRolesViewModel.RoleName = role.Name;

                    if (userRole != null)
                    {
                        userRolesViewModel.StartDate = userRole.StartDate;
                        userRolesViewModel.EndDate = userRole.EndDate;
                    }

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
            AssignRoleList = model;
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

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> AssignRoleList, string id, string returnUrl = null)
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

                    foreach (var selectedRoles in AssignRoleList.Where(x => x.Selected))
                    {
                        var role = await _roleManager.FindByNameAsync(selectedRoles.RoleName);
                        ApplicationUserRole userRole = new()
                        {
                            RoleId = selectedRoles.RoleId,
                            UserId = user.Id,
                            ApplicationUser = user,
                            ApplicationRole = role,
                            StartDate = selectedRoles.StartDate,
                            EndDate = selectedRoles.EndDate,
                        };

                        await _db.UserRoles.AddAsync(userRole);
                    }

                    await _userManager.UpdateAsync(user);
                    _db.SaveChanges();
                    await LoadAsync(user);
                    StatusMessage = "Successfully created user profile";
                    return Page();
                }
            }
            return Page();
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}

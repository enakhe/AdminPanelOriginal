#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Admin.Pages.User
{
    [Authorize(Roles = "SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public string ReturnUrl { get; set; }
        public IList<UserRolesViewModel> UserRoleList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet(string statusMessage, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var users = await _userManager.Users.Where(user => !user.UserName.Contains("SuperAdmin")).ToListAsync();
                var userRolesViewModel = new List<UserRolesViewModel>();

                foreach (ApplicationUser user in users)
                {
                    var thisViewModel = new UserRolesViewModel();
                    thisViewModel.Id = user.Id;
                    thisViewModel.Username = user.UserName;
                    thisViewModel.Email = user.Email;
                    thisViewModel.FullName = user.FullName;
                    thisViewModel.DateCreated = user.DateCreated;
                    thisViewModel.Roles = await GetUserRoles(user);
                    userRolesViewModel.Add(thisViewModel);
                }
                StatusMessage = statusMessage;
                UserRoleList = userRolesViewModel;
            }
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}

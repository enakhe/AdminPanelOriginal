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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
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
                    LogsInfo logsInfo = await _db.LogsInfos.FirstOrDefaultAsync(logsInfo => logsInfo.UserId == user.Id);

                    var thisViewModel = new UserRolesViewModel();
                    thisViewModel.Id = user.Id;
                    thisViewModel.Username = user.UserName;
                    thisViewModel.Email = user.Email;
                    thisViewModel.FullName = user.FullName;
                    thisViewModel.ProfilePicture = user.ProfilePicture;
                    if (logsInfo != null)
                    {
                        thisViewModel.DateCreated = logsInfo.DateCreated;
                    }
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

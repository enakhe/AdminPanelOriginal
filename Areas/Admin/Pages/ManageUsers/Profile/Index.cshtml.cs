#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.ManageUsers.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationUser UserData { get; set; }
        public byte[] UserProfilePicture { get; set; }

        public string ReturnUrl { get; set; }

        public void LoadAsync(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                UserData = user;
                UserProfilePicture = user.ProfilePicture;
            }
        }
        public async Task OnGet(string id, string returnUrl)
        {
            returnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    LoadAsync(user);
                }
            }
        }
    }
}

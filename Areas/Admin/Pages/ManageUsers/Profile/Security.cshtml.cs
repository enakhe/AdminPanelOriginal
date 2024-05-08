#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Interface;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AdminPanel.Areas.Admin.Pages.ManageUsers.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class SecurityModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IAuditLog _auditLog;


        public SecurityModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db, SignInManager<ApplicationUser> signInManager, IAuditLog auditLog)
        {
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
            _auditLog = auditLog;
        }

        [BindProperty]
        public UserPersonalInfoInputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationUser UserData { get; set; }
        public byte[] UserProfilePicture { get; set; }
        public string Code { get; set; }

        public void LoadAsync(ApplicationUser user)
        {
            UserData = user;
            UserProfilePicture = user.ProfilePicture;
        }

        public async Task OnGetAsync(string id)
        {
            _ = ReturnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    LoadAsync(user);
                }
            }
            else
            {
                StatusMessage = "Error, something unexpected happened";
            }
        }

        public async Task<IActionResult> OnPostAsync(ApplicationUser user, string newPassword)
        {
            _ = ReturnUrl;
            ApplicationUser updatedUser = await _userManager.FindByIdAsync(user.Id);

            await _userManager.ChangePasswordAsync(updatedUser, "DummyUser$01", newPassword);
            await _userManager.UpdateAsync(updatedUser);
            StatusMessage = "Successfully created user profile";

            LoadAsync(user);
            return Page();
        }
    }
}

#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Interface;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> OnPostAsync(string id, string password)
        {
            var user = await _userManager.FindByIdAsync(id);
            var admin = await _db.Users.FirstOrDefaultAsync(user => user.UserName == "SuperAdmin");


            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var currentPassword = await _userManager.CheckPasswordAsync(user, password);
            if (currentPassword)
            {
                StatusMessage = "Error, you cannot use the same current password";
                await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                LoadAsync(user);
                return Page();
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, password);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    StatusMessage = error.Description;
                    await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                }
                LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been set";
            await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);

            LoadAsync(user);
            return RedirectToPage();
        }
    }
}

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

        public async void LoadAsync(ApplicationUser user)
        {
            
        }

        public async Task OnGetAsync(string id)
        {
            
        }

        public async Task<IActionResult> OnPostAsync(string id, string newPassword)
        {
            return Page();
        }
    }
}

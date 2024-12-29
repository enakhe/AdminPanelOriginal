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
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SecurityModel> _logger;


        public SecurityModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db, SignInManager<ApplicationUser> signInManager, IAuditLog auditLog, ILogger<SecurityModel> logger)
        {
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
            _auditLog = auditLog;
            _logger = logger;
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

        public async Task<IActionResult> OnPostAsync(string Id, string newPassword)
        {
            _ = ReturnUrl;
            ApplicationUser admin = await _userManager.GetUserAsync(User);
            ApplicationUser user = await _userManager.FindByIdAsync(Id);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if(result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
                StatusMessage = "Successfully updated user password";

                // Add Audit Device Information
                var continent = await _auditLog.GetContinent();
                var countryName = await _auditLog.GetCountryName();
                var country = await _auditLog.GetCountry();
                var city = await _auditLog.GetCity();
                var state = await _auditLog.GetState();
                AuditDeviceInfo auditDeviceInfo = new()
                {
                    DeviceType = _auditLog.GetDeviceType(HttpContext),
                    OperatingSystem = _auditLog.GetOperatingSystem(HttpContext),
                    BrowserName = _auditLog.GetBrowserName(HttpContext),
                    BrowserVersion = _auditLog.GetBrowserVersion(HttpContext),
                    IPAddress = _auditLog.GetIpAddress(HttpContext),
                    DeviceContinent = continent,
                    DeviceCountryName = countryName,
                    DeviceCountry = country,
                    DeviceCity = city,
                    DeviceState = state,
                };
                await _db.AuditDeviceInfo.AddAsync(auditDeviceInfo);

                // Add Audit Loggin Information
                AuditLogging auditLogging = new()
                {
                    AdminId = admin.Id,
                    User = user,
                    UserId = user.Id,
                    DeviceInfoId = auditDeviceInfo.Id,
                    AuditDeviceInfo = auditDeviceInfo,
                    AuditActionType = "Post",
                    StatusMessage = StatusMessage
                };
                await _db.AuditLoggings.AddAsync(auditLogging);

                await _db.SaveChangesAsync();

                LoadAsync(user);
            } 
            else
            {
                var error = result.Errors.ToList()[0].Description;
                _logger.LogError(error);
                StatusMessage = $"Error, {error}";
                LoadAsync(user);
                return Page();
            }

            LoadAsync(user);
            return Page();
        }
    }
}

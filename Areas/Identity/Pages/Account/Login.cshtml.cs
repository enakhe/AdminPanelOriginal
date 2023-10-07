#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Interface;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LoginModel> _logger;
        private readonly IAuditLog _auditLog;


        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext db, IAuditLog auditLog)
        {
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _auditLog = auditLog;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public LoginInputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var userName = Input?.Username;
                var user = await _userManager.FindByNameAsync(Input.Username);
                var admin = await _db.Users.FirstOrDefaultAsync(user => user.UserName == "SuperAdmin");
                if (user != null)
                {
                    userName = user.UserName;
                }
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var currentUser = await _userManager.FindByNameAsync(Input.Username);
                    var roles = await _userManager.GetRolesAsync(currentUser);
                    PersonalizationInfo personalizationInfo = await _db.PersonalizationInfos.FirstOrDefaultAsync(personalizationInfo => personalizationInfo.UserId == user.Id);

                    if (roles.Contains("SuperAdmin"))
                    {
                        return RedirectToPage("/Dashboard/Index", new { area = "Admin" });
                    }
                    else
                    {
                        if (personalizationInfo != null)
                        {
                            if (personalizationInfo.IsAuthorized == false)
                            {
                                StatusMessage = "Error, you are not authorized to have access";

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
                                await _signInManager.SignOutAsync();
                                return Page();
                            }
                            else
                            {
                                StatusMessage = "Successfully logged in";

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
                                return RedirectToPage("/Dashboard/Index", new { area = "User" });
                            }
                        }
                        else
                        {
                            StatusMessage = "Error, you are not authorized to have access";
                            await _signInManager.SignOutAsync();
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
                            return Page();
                        }
                    }
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    StatusMessage = "Error, Invalid login attempt";
                    return Page();
                }
            }
            return Page();
        }
    }
}

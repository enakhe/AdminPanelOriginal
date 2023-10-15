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
                else
                {
                    StatusMessage = "Error, user with the username does not exit";
                    return Page();
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
                                await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                                await _signInManager.SignOutAsync();
                                return Page();
                            }
                            else
                            {
                                StatusMessage = "Successfully logged in";
                                await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                                return RedirectToPage("/Dashboard/Index", new { area = "User" });
                            }
                        }
                        else
                        {
                            StatusMessage = "Error, you are not authorized to have access";
                            _auditLog.AddAudit(HttpContext, admin, user, StatusMessage); 
                            await _db.SaveChangesAsync();
                            await _signInManager.SignOutAsync();
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
                    StatusMessage = "Error, incorrect password";
                    return Page();
                }
            }
            return Page();
        }
    }
}

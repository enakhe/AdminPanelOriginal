#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using AdminPanel.Models;
using AdminPanel.InputModel;
using AdminPanel.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
            _userManager = userManager;
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
                                await _signInManager.SignOutAsync();
                                return Page();
                            }
                            else
                            {
                                return RedirectToPage("/Dashboard/Index", new { area = "User" });
                            }
                        }
                        else
                        {
                            StatusMessage = "Error, you are not authorized to have access";
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
                    StatusMessage = "Error, Invalid login attempt";
                    return Page();
                }
            }
            return Page();
        }
    }
}

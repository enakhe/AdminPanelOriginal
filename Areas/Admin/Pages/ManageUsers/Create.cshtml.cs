#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdminPanel.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using AdminPanel.Data;
using AdminPanel.ViewModels;

namespace AdminPanel.Areas.Admin.Pages.User
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<CreateModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        public readonly ApplicationDbContext _db;

        public CreateModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<CreateModel> logger,
            IEmailSender emailSender, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<ManageUserRolesViewModel> RoleList { get; set; }


        public void OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                if (!role.Name.Contains("SuperAdmin"))
                {
                    var userRolesViewModel = new ManageUserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };
                    model.Add(userRolesViewModel);
                }
            }
            RoleList = model;
        }

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> RoleList, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                int year = DateTime.Now.Year;
                var user = new ApplicationUser();

                user.UserName = string.Concat("ADPA", user.Id.ToString().AsSpan(0, 3), year.ToString().ToUpper());
                user.FirstName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper());
                user.LastName = Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                user.FullName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper()) + " " + Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());

                using (var dataStream = new MemoryStream())
                {
                    await Input.ProfilePicture.CopyToAsync(dataStream);
                    if (dataStream.Length < 2097152)
                    {
                        user.ProfilePicture = dataStream.ToArray();
                        await _userManager.UpdateAsync(user);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large");
                    }
                }

                await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, "DummyUser$01");

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var result2 = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot remove user existing roles");
                        return Page();
                    }
                    result2 = await _userManager.AddToRolesAsync(user, RoleList.Where(x => x.Selected).Select(y => y.RoleName));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot add selected roles to user");
                        return Page();
                    }

                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("Admin created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using AdminPanel.Models;
using AdminPanel.InputModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using AdminPanel.Data;

namespace AdminPanel.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, ApplicationDbContext db)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = Input.UserName;

                await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    user.UserName = Input.UserName;

                    // Add Personal Info
                    user.FirstName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper());
                    user.LastName = Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                    user.FullName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper()) + " " + Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                    user.Gender = Input.Gender;
                    user.PhoneNumber = Input.PhoneNumber;

                    // Add Contact Info
                    ContactInfo contactInfo = new()
                    {
                        Street = "",
                        City = "",
                        State = "",
                        ZipCode = "",

                        UserId = user.Id
                    };
                    await _db.ContactInfos.AddAsync(contactInfo);

                    // Add Personalization Info
                    PersonalizationInfo personalizationInfo = new()
                    {
                        IsAuthorized = false,
                        IsOnline = false,

                        UserId = user.Id
                    };
                    await _db.PersonalizationInfos.AddAsync(personalizationInfo);

                    // Add Logs Info
                    LogsInfo logsInfo = new()
                    {
                        LastLogin = DateTime.Now,
                        DateUpdated = DateTime.Now,

                        UserId = user.Id
                    };
                    await _db.LogsInfos.AddAsync(logsInfo);
                    await _userManager.UpdateAsync(user);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        return LocalRedirect(returnUrl);
                    }
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

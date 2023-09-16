#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

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
        private readonly RoleManager<ApplicationRole> _roleManager;
        public readonly ApplicationDbContext _db;

        public CreateModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<CreateModel> logger,
            IEmailSender emailSender, ApplicationDbContext db, RoleManager<ApplicationRole> roleManager)
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

        [TempData]
        public string StatusMessage { get; set; }

        public IList<ManageUserRolesViewModel> RoleList { get; set; }

        public void LoadAsync()
        {
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

        public void OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                LoadAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> RoleList, string returnUrl = null)
        {
            returnUrl ??= Url.Page("/ManageUsers/Index");
            if (ModelState.IsValid)
            {
                LoadAsync();

                var user = new ApplicationUser();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = Input.UserName;

                await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, "DummyUser$01");

                if (result.Succeeded)
                {
                    user.UserName = Input.UserName;

                    // Add Personal Info
                    user.FirstName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper());
                    user.LastName = Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                    user.FullName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper()) + " " + Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                    user.DOB = Input.DOB;
                    user.Gender = Input.Gender;
                    user.PhoneNumber = Input.PhoneNumber;

                    // Add Profile Picture
                    if (Input.ProfilePicture != null)
                    {
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
                    }

                    // Add Contact Info
                    ContactInfo contactInfo = new()
                    {
                        Street = Input.Street,
                        City = Input.City,
                        State = Input.State,
                        ZipCode = Input.ZipCode,

                        UserId = user.Id
                    };
                    await _db.ContactInfos.AddAsync(contactInfo);

                    // Add Personalization Info
                    PersonalizationInfo personalizationInfo = new()
                    {
                        IsAuthorized = Input.IsAuthorized,
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


                    // Assign Roles
                    // var roles = await _userManager.GetRolesAsync(user);
                    //var roleResult = await _userManager.AddToRolesAsync(user, RoleList.Where(x => x.Selected).Select(y => y.RoleName));

                    foreach (var selectedRoles in RoleList.Where(x => x.Selected))
                    {
                        var role = await _roleManager.FindByNameAsync(selectedRoles.RoleName);
                        ApplicationUserRole userRole = new()
                        {
                            RoleId = selectedRoles.RoleId,
                            UserId = user.Id,
                            ApplicationUser = user,
                            ApplicationRole = role,
                            StartDate = selectedRoles.StartDate,
                            EndDate = selectedRoles.EndDate,
                        };

                        await _db.UserRoles.AddAsync(userRole);
                    }

                    await _userManager.UpdateAsync(user);
                    _db.SaveChanges();
                    StatusMessage = "Successfully created user profile";
                    return RedirectToPage("/ManageUsers/Index", new { area = "Admin", statusMessage = StatusMessage });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
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

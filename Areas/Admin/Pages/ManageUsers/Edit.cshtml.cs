#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AdminPanel.Areas.Admin.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public RegisterInputModel Input { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        public IList<ManageUserRolesViewModel> RoleList { get; set; }

        public byte[] UserProfilePicture { get; set; }

        public ApplicationUser UserData { get; set; }

        public void LoadAsync(ApplicationUser user)
        {
            Input = new RegisterInputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            UserProfilePicture = user.ProfilePicture;
            UserData = user;
        }

        public async Task OnGet(string id, string returnUrl)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    LoadAsync(user);
                    UserData = user;

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
                            if (await _userManager.IsInRoleAsync(user, role.Name))
                            {
                                userRolesViewModel.Selected = true;
                            }
                            else
                            {
                                userRolesViewModel.Selected = false;
                            }
                            model.Add(userRolesViewModel);
                        }
                    }
                    RoleList = model;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> RoleList, string id, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = await _userManager.FindByIdAsync(id);
                LoadAsync(user);

                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{user.Id}'.");
                }

                if (!ModelState.IsValid)
                {
                    LoadAsync(user);
                    return Page();
                }


                string firstName = user.FirstName;
                string lastName = user.LastName;
                string email = user.Email;
                byte[] profileImage = user.ProfilePicture;

                if (Input.FirstName != firstName)
                {
                    user.FirstName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper());
                    _ = await _userManager.UpdateAsync(user);
                }

                if (Input.LastName != lastName)
                {
                    user.LastName = Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                    _ = await _userManager.UpdateAsync(user);
                }

                user.FullName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper()) + " " + Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());

                if (Input.Email != email)
                {
                    IdentityResult setEmail = await _userManager.SetEmailAsync(user, Input.Email);
                    if (!setEmail.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return RedirectToPage();
                    }
                }

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

                var roles = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove user existing roles");
                    return Page();
                }
                result = await _userManager.AddToRolesAsync(user, RoleList.Where(x => x.Selected).Select(y => y.RoleName));
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected roles to user");
                    return Page();
                }


                _ = await _userManager.UpdateAsync(user);
                StatusMessage = "User profile has been updated";
                return RedirectToPage("/Users/Index", new { area = "Admin", statusMessage = StatusMessage });
            }
            StatusMessage = "Error, something unexpected happened";
            return Page();
        }
    }
}

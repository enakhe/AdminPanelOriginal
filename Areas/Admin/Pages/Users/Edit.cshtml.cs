#nullable disable

using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;

namespace AdminPanel.Areas.Admin.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public RegisterInputModel Input { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        public IList<ManageUserRolesViewModel> RoleList { get; set; }

        public ApplicationUser UserData { get; set; }

        public void LoadAsync(ApplicationUser user)
        {
            Input = new RegisterInputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

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
                    RoleList = model;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> RoleList)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                 LoadAsync(user);
                return Page();
            }

            string firstName = user.FirstName;
            string lastName = user.LastName;
            string email = user.Email;

            if (Input.FirstName != firstName)
            {
                user.FirstName = Input.FirstName;
                _ = await _userManager.UpdateAsync(user);
            }

            if (Input.LastName != lastName)
            {
                user.LastName = Input.LastName;
                _ = await _userManager.UpdateAsync(user);
            }


            if (Input.Email != email)
            {
                IdentityResult setEmail = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmail.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
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
    }
}

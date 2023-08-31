#nullable disable

using AdminPanel.InputModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.Roles
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public CreateModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string roleName, string returnUrl = null)
        {
            if(ModelState.IsValid)
            {
                if (roleName != null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
                }
                else
                {
                    StatusMessage = "Error, input required field";
                    return RedirectToPage("/Roles/Index", new { area = "Admin", statusMessage = StatusMessage });
                }
            }
            StatusMessage = "Successfully added role";
            return RedirectToPage("/Roles/Index", new { area = "Admin", statusMessage = StatusMessage });
        }
    }
}

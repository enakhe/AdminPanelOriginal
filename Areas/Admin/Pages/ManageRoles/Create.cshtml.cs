#nullable disable

using AdminPanel.Enum;
using AdminPanel.InputModel;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Areas.Admin.Pages.Roles
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public CreateModel(RoleManager<ApplicationRole> roleManager)
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
                    ApplicationRole defaultRole = new()
                    {
                        Name = roleName.Trim()
                    };
                    defaultRole.Id = Guid.NewGuid().ToString();
                    await _roleManager.CreateAsync(defaultRole);
                }
                else
                {
                    StatusMessage = "Error, input required field";
                    return RedirectToPage("/ManageRoles/Index", new { area = "Admin", statusMessage = StatusMessage });
                }
            }
            StatusMessage = "Successfully added role";
            return RedirectToPage("/ManageRoles/Index", new { area = "Admin", statusMessage = StatusMessage });
        }
    }
}

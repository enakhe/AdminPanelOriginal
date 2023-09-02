#nullable disable

using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Admin.Pages.Roles
{
    [Authorize(Roles = "SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public IndexModel(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public string ReturnUrl { get; set; }
        public IList<ApplicationRole> RoleList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet(string statusMessage, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                StatusMessage = statusMessage;
                RoleList = await _roleManager.Roles.Where(role => !role.Name.Contains("SuperAdmin")).ToListAsync();
            }
        }
    }
}

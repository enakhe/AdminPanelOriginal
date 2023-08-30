#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AdminPanel.Areas.Admin.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class RoleModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly ApplicationDbContext _db;


        public RoleModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public string ReturnUrl { get; set; }

        public async Task OnGet(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                ApplicationUser thisCreatedUser = await _db.Users.FirstOrDefaultAsync(createdUser => createdUser.Id == id);



                
            }
            return Page();
        }
    }
}

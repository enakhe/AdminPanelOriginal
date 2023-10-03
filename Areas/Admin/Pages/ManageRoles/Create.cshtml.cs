#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AdminPanel.Areas.Admin.Pages.Roles
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public readonly ApplicationDbContext _db;

        public CreateModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public RoleInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<ApplicationUser> ApplicationUserList { get; set; }

        public async Task LoadAsync()
        {
            if (ModelState.IsValid)
            {
                var allUser = await _db.Users.OrderBy(user => user.FirstName).ToListAsync();
                if (allUser != null)
                {
                    ApplicationUserList = allUser;
                }
            }
        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                await LoadAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync(string ManagerId, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {

                ApplicationRole role = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Regex.Replace(Input.Name, "^[a-z]", c => c.Value.ToUpper()),
                    NormalizedName = Input.Name.ToUpper(),
                    Description = Input.Description,
                    Tag = Input.Tag,
                    ExpirationDate = Input.ExpirationDate,

                    ManagerId = ManagerId,
                    Manager = _db.Users.FirstOrDefault(user => user.Id == ManagerId)
                };

                // Add Icon
                if (Input.Icon != null)
                {
                    using (var dataStream = new MemoryStream())
                    {
                        await Input.Icon.CopyToAsync(dataStream);
                        if (dataStream.Length < 2097152)
                        {
                            role.Icon = dataStream.ToArray();
                        }
                        else
                        {
                            ModelState.AddModelError("File", "The file is too large");
                        }
                    }
                }

                await _db.Roles.AddAsync(role);
                await _db.SaveChangesAsync();

                StatusMessage = "Successfully added role";
                await LoadAsync();
                return RedirectToPage("/ManageRoles/Index", new { area = "Admin" });
            }
            await LoadAsync();
            return Page();
        }
    }
}

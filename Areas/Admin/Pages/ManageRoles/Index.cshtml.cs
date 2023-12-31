#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
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
        private readonly ApplicationDbContext _db;
        public IndexModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _db = db;
        }

        public string ReturnUrl { get; set; }
        public IList<ApplicationRole> RoleList { get; set; }
        public int NoOfUser { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                RoleList = await _roleManager.Roles.Where(role => !role.Name.Contains("SuperAdmin")).Include(role => role.Manager).ToListAsync();

                foreach (var role in RoleList)
                {
                    var NoOfUser = _db.UserRoles.Where(userRole => userRole.RoleId == role.Id).Count();
                    var userRole = _db.UserRoles.FirstOrDefault(userRole => userRole.RoleId == role.Id);

                    if (userRole != null)
                    {
                        var thisRole = _roleManager.Roles.FirstOrDefault(role => role.Id == userRole.RoleId);
                        if (thisRole != null)
                        {
                            thisRole.NoOfUser = NoOfUser;
                            await _roleManager.UpdateAsync(thisRole);
                        }
                    }
                    else if (NoOfUser == 0)
                    {
                        var thisRole = _roleManager.Roles.FirstOrDefault(r => r.Id == role.Id);
                        thisRole.NoOfUser = NoOfUser;
                        await _roleManager.UpdateAsync(thisRole);
                    }
                }
            }
        }
    }
}

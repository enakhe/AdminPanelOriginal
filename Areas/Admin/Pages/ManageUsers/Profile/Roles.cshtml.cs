#nullable disable

using AdminPanel.Data;
using AdminPanel.Interface;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Admin.Pages.ManageUsers.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class RolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IAuditLog _auditLog;


        public RolesModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext db, IAuditLog auditLog)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _auditLog = auditLog;
        }

        public string ReturnUrl { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationUser UserData { get; set; }
        public IList<RoleViewModel> RoleList { get; set; }
        public IList<ManageUserRolesViewModel> AssignRoleList { get; set; }
        public UserBackUpInfo UserBackUpInfo { get; set; }
        public byte[] UserProfilePicture { get; set; }

        public async Task LoadAsync(ApplicationUser user)
        {
            UserData = user;
            UserProfilePicture = user.ProfilePicture;


            // Loading all the roles a user belongs to
            var userRoles = await _db.UserRoles.Where(userRole => userRole.UserId == user.Id).Include(userRole => userRole.ApplicationRole).ToListAsync();
            var roleModel = new List<RoleViewModel>();
            if (userRoles != null)
            {
                foreach (var userRole in userRoles)
                {
                    var role = await _db.Roles.FirstOrDefaultAsync(ur => ur.Id == userRole.RoleId);

                    var roleViewModel = new RoleViewModel();
                    roleViewModel.RoleName = role.Name;
                    roleViewModel.StartDate = userRole.StartDate;
                    roleViewModel.EndDate = userRole.EndDate;

                    if (userRole.StartDate > DateTime.Now || userRole.EndDate < DateTime.Now)
                    {
                        roleViewModel.isActive = false;
                    }
                    else
                    {
                        roleViewModel.isActive = true;
                        userRole.isActive = true;
                        _db.Entry(userRole).State = EntityState.Modified;

                        roleViewModel.DaysLeft = userRole.EndDate - DateTime.Now;
                    }
                    roleModel.Add(roleViewModel);
                }
                RoleList = roleModel;
            }



            // Loading all roles 
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                if (!role.Name.Contains("SuperAdmin"))
                {
                    var userRole = await _db.UserRoles.FirstOrDefaultAsync(userRole => userRole.UserId == user.Id);

                    var userRolesViewModel = new ManageUserRolesViewModel();

                    userRolesViewModel.RoleId = role.Id;
                    userRolesViewModel.RoleName = role.Name;

                    if (userRole != null)
                    {
                        userRolesViewModel.StartDate = userRole.StartDate;
                        userRolesViewModel.EndDate = userRole.EndDate;
                    }

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
            AssignRoleList = model;
        }

        public async Task OnGetAsync(string id, string returnUrl = null)
        {
            returnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await LoadAsync(user);
                }
            }
            else
            {
                StatusMessage = "Error, something unexpected happened";
            }
        }

        public async Task<IActionResult> OnPostAsync(List<ManageUserRolesViewModel> AssignRoleList, string id, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                var admin = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var result = await _userManager.RemoveFromRolesAsync(user, roles);

                    if (!result.Succeeded)
                    {
                        StatusMessage = "Error, cannot remove user existing roles";

                        // Add Audit Device Information
                        var continent = await _auditLog.GetContinent();
                        var countryName = await _auditLog.GetCountryName();
                        var country = await _auditLog.GetCountry();
                        var city = await _auditLog.GetCity();
                        var state = await _auditLog.GetState();
                        AuditDeviceInfo auditDeviceInfo = new()
                        {
                            DeviceType = _auditLog.GetDeviceType(HttpContext),
                            OperatingSystem = _auditLog.GetOperatingSystem(HttpContext),
                            BrowserName = _auditLog.GetBrowserName(HttpContext),
                            BrowserVersion = _auditLog.GetBrowserVersion(HttpContext),
                            IPAddress = _auditLog.GetIpAddress(HttpContext),
                            DeviceContinent = continent,
                            DeviceCountryName = countryName,
                            DeviceCountry = country,
                            DeviceCity = city,
                            DeviceState = state,
                        };
                        await _db.AuditDeviceInfo.AddAsync(auditDeviceInfo);

                        // Add Audit Loggin Information
                        AuditLogging auditLogging = new()
                        {
                            AdminId = admin.Id,
                            User = user,
                            UserId = user.Id,
                            DeviceInfoId = auditDeviceInfo.Id,
                            AuditDeviceInfo = auditDeviceInfo,
                            AuditActionType = "Post",
                            StatusMessage = StatusMessage
                        };
                        await _db.AuditLoggings.AddAsync(auditLogging);
                        await _db.SaveChangesAsync();
                        return Page();
                    }

                    foreach (var selectedRoles in AssignRoleList.Where(x => x.Selected))
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

                        StatusMessage = $"Successfully assigned user to {role.Name} role";

                        // Add Audit Device Information
                        var continent = await _auditLog.GetContinent();
                        var countryName = await _auditLog.GetCountryName();
                        var country = await _auditLog.GetCountry();
                        var city = await _auditLog.GetCity();
                        var state = await _auditLog.GetState();
                        AuditDeviceInfo auditDeviceInfo = new()
                        {
                            DeviceType = _auditLog.GetDeviceType(HttpContext),
                            OperatingSystem = _auditLog.GetOperatingSystem(HttpContext),
                            BrowserName = _auditLog.GetBrowserName(HttpContext),
                            BrowserVersion = _auditLog.GetBrowserVersion(HttpContext),
                            IPAddress = _auditLog.GetIpAddress(HttpContext),
                            DeviceContinent = continent,
                            DeviceCountryName = countryName,
                            DeviceCountry = country,
                            DeviceCity = city,
                            DeviceState = state,
                        };
                        await _db.AuditDeviceInfo.AddAsync(auditDeviceInfo);

                        // Add Audit Loggin Information
                        AuditLogging auditLogging = new()
                        {
                            AdminId = admin.Id,
                            User = user,
                            UserId = user.Id,
                            DeviceInfoId = auditDeviceInfo.Id,
                            AuditDeviceInfo = auditDeviceInfo,
                            AuditActionType = "Post",
                            StatusMessage = StatusMessage
                        };
                        await _db.AuditLoggings.AddAsync(auditLogging);
                        await _db.SaveChangesAsync();
                    }

                    await _userManager.UpdateAsync(user);
                    await _db.SaveChangesAsync();
                    await LoadAsync(user);

                    StatusMessage = "Successfully assined role to user";
                    return Page();
                }
            }
            return Page();
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}

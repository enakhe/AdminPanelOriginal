#nullable disable

using AdminPanel.Enum;
using AdminPanel.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            //Seed Roles
            ApplicationRole defaultRole = new()
            {
                Name = DefaultRoles.Roles.SuperAdmin.ToString()
            };

            if (roleManager.Roles.All(r => r.Id == defaultRole.Id))
            {
                var role = await roleManager.FindByIdAsync(defaultRole.Id);
                if (role == null)
                {
                    defaultRole.Id = Guid.NewGuid().ToString();
                    defaultRole.DateCreated = DateTime.Now;
                    await roleManager.CreateAsync(defaultRole);
                }
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "SuperAdmin",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                FirstName = "SAMUEL",
                FullName = "SAMUEL " + "ENAKHE " + "IZUAGBE",
                LastName = "IZUAGBE",
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    defaultUser.Id = Guid.NewGuid().ToString();
                    await userManager.CreateAsync(defaultUser, "Samcooper$01");
                    await userManager.AddToRoleAsync(defaultUser, DefaultRoles.Roles.SuperAdmin.ToString());
                }

            }
        }
    }
}

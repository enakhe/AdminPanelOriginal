#nullable disable

using AdminPanel.Model;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(ApplicationRole.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(ApplicationRole.Roles.Student.ToString()));

        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "SuperAdmin",
                Email = "admin@gmail.com",
                FirstName = "SAMUEL",
                FullName = "SAMUEL " + "ENAKHE " + "IZUAGBE",
                LastName = "IZUAGBE",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Samcooper$01");
                    await userManager.AddToRoleAsync(defaultUser, ApplicationRole.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, ApplicationRole.Roles.Student.ToString());
                }

            }
        }
    }
}

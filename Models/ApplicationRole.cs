using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationRole : IdentityRole
    {
        public enum Roles
        {
            SuperAdmin,
        }
    }
}
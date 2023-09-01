using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Enum
{
    public class DefaultRoles : IdentityRole
    {
        public enum Roles
        {
            SuperAdmin,
        }
    }
}
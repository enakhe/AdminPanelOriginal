using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Data
{
    public class ApplicationRole : IdentityRole
    {
        public enum Roles
        {
            Admin,
            Student
        }
    }
}
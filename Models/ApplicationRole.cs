using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationRole : IdentityRole
    {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public enum Roles
        {
            SuperAdmin,
        }
    }
}
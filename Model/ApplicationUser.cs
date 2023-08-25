#nullable disable

using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int UsernameChangeLimit { get; set; } = 10;
    }
}

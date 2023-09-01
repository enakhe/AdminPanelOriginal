#nullable disable

using AdminPanel;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;

        public bool isAuthorized { get; set; }
        public bool isOnline { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastLogin { get; set; }

    }
}

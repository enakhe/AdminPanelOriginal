using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public DateTime DateCreated { get; set; }
    }
}

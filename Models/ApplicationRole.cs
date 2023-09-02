using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public int NoOfUser { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

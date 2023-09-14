using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() 
        {
            this.JoinEntities = new HashSet<ApplicationUserRole>();
        }
        public int NoOfUser { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ICollection<ApplicationUserRole> JoinEntities { get; set; }
    }
}

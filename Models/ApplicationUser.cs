#nullable disable

using AdminPanel;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace AdminPanel.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser() 
        {
            this.Contact = new HashSet<ContactInfo>();
            this.PersonalInfo = new HashSet<PersonalInfo>();
            this.Personalization = new HashSet<PersonalizationInfo>();
            this.Logs = new HashSet<LogsInfo>();


        }

        public virtual ICollection<ContactInfo> Contact { get; set; }
        public virtual ICollection<PersonalInfo> PersonalInfo { get; set; }
        public virtual ICollection<PersonalizationInfo> Personalization { get; set; }
        public virtual ICollection<LogsInfo> Logs { get; set; }

    }
}

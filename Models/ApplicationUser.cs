#nullable disable

using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser()
        {
            this.Contact = new HashSet<ContactInfo>();
            this.Personalization = new HashSet<PersonalizationInfo>();
            this.Logs = new HashSet<LogsInfo>();
            this.BackUp = new HashSet<UserBackUpInfo>();
        }

        //Basic Infromation
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;


        public virtual ICollection<ContactInfo> Contact { get; set; }
        public virtual ICollection<PersonalizationInfo> Personalization { get; set; }
        public virtual ICollection<LogsInfo> Logs { get; set; }
        public virtual ICollection<UserBackUpInfo> BackUp { get; set; }

    }
}

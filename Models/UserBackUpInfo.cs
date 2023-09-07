#nullable disable

namespace AdminPanel.Models
{
    public class UserBackUpInfo
    {
        public string Id { get; set; }
        public string BackupEmail { get; set; }
        public string BackupPhoneNumber { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

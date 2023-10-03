#nullable disable

namespace AdminPanel.Models
{
    public class AuditDeviceInfo
    {
        public AuditDeviceInfo() 
        {
            this.AuditLoggings = new HashSet<AuditLogging>();
        }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DeviceID { get; set; } = Guid.NewGuid().ToString();
        public string DeviceType { get; set; }
        public string OperatingSystem { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string IPAddress { get; set; }
        public string DeviceLocation { get; set; }
        public string DeviceOwner { get; set; }

        public virtual ICollection<AuditLogging> AuditLoggings { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    }
}

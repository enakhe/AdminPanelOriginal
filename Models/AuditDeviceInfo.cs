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
        public string DeviceContinent { get; set; }
        public string DeviceCountryName { get; set; }
        public string DeviceCountry { get; set; }
        public string DeviceState { get; set; }
        public string DeviceCity { get; set; }

        public virtual ICollection<AuditLogging> AuditLoggings { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    }
}

#nullable disable

namespace AdminPanel.Models
{
    public class AuditLogging
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AdminId { get; set; }
        public string UserId { get; set; }
        public string TypeId { get; set; }
        public string DeviceInfoId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual AuditActionType AuditActionType { get; set; }
        public virtual AuditDeviceInfo AuditDeviceInfo { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}

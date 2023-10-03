#nullable disable

namespace AdminPanel.Models
{
    public class AuditActionType
    {
        public AuditActionType() 
        {
            this.AuditLoggings = new HashSet<AuditLogging>();
        }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }

        public virtual ICollection<AuditLogging> AuditLoggings { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}

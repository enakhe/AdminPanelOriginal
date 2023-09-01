#nullable disable

namespace AdminPanel.Models
{
    public class LogsInfo
    {
        //Logs Infromation
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastLogin { get; set; }
        public DateTime DateUpdated { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

#nullable disable

namespace AdminPanel.Models
{
    public class ApplicationRoleCategory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CategoryId { get; set; }
        public string RoleId { get; set; }

        public virtual ApplicationCategory Category { get; set; }
        public virtual ApplicationRole ApplicationRole { get; set; }

    }
}

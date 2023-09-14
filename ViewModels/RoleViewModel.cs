#nullable disable

namespace AdminPanel.ViewModels
{
    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool isActive { get; set; } = false;
        public TimeSpan DaysLeft { get; set; }
    }
}

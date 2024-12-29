#nullable disable

namespace AdminPanel.Models
{
    public class PersonalizationInfo
    {
        //Personalization Infromation
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsAuthorized { get; set; }
        public bool IsDisabled { get; set; } = false;
        public bool IsOnline { get; set; } = false;

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

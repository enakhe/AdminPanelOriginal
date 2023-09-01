#nullable disable

namespace AdminPanel.Models
{
    public class PersonalizationInfo
    {
        //Personalization Infromation
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsAuthorized { get; set; }
        public bool IsOnline { get; set; } = false;
    }
}

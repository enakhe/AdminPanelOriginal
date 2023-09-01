#nullable disable

namespace AdminPanel.Models
{
    public class ContactInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string HomeAddress { get; set; }
        public string WorkAddress { get; set; }
        public string OtherAddress { get; set; }
    }
}

#nullable disable

namespace AdminPanel.Models
{
    public class ContactInfo
    {
        //Contact Infromation
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string HomeAddress { get; set; }
        public string WorkAddress { get; set; }
        public string OtherAddress { get; set; }
        public string ZipCode { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

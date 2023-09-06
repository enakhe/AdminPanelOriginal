#nullable disable

namespace AdminPanel.Models
{
    public class ContactInfo
    {
        //Contact Infromation
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Street{ get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}

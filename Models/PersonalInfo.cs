#nullable disable

namespace AdminPanel.Models
{
    public class PersonalInfo
    {
        //Basic Infromation
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
    }
}

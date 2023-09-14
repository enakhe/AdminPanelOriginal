#nullable disable


namespace AdminPanel.ViewModels
{
    public class UserRolesViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[] ProfilePicture { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}

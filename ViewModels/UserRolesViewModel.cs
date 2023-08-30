#nullable disable

using Microsoft.CodeAnalysis.Elfie.Model.Strings;

namespace AdminPanel.ViewModels
{
    public class UserRolesViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}

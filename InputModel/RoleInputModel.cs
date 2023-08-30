#nullable disable

using System.ComponentModel.DataAnnotations;

namespace AdminPanel.InputModel
{
    public class RoleInputModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}

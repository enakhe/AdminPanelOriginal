#nullable disable

using System.ComponentModel.DataAnnotations;

namespace AdminPanel.InputModel
{
    public class RoleInputModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Icon")]
        public IFormFile Icon { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Tag")]
        public string Tag { get; set; }

        [Required]
        [Display(Name = "Status")]
        public bool Status { get; set; }

        [Required]
        [Display(Name = "ExpirationDate")]
        public DateTime ExpirationDate { get; set; }


    }
}

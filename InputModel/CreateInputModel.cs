#nullable disable

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel
{
    public class CreateInputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [PersonalData]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "Profile Picture")]
        public string Street { get; set; }

        [PersonalData]
        [Display(Name = "Work Address")]
        public string City { get; set; }

        [PersonalData]
        [Display(Name = "OtherAddress")]
        public string State { get; set; }

        [PersonalData]
        [Display(Name = "ZipCode")]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Authorize")]
        public bool IsAuthorized { get; set; }
    }
}

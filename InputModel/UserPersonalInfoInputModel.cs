#nullable disable

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.InputModel
{
    public class UserPersonalInfoInputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [PersonalData]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [PersonalData]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [PersonalData]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [PersonalData]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        [PersonalData]
        [Display(Name = "Profile Picture")]
        public string Street { get; set; }

        [PersonalData]
        [Display(Name = "Work Address")]
        public string City { get; set; }

        [PersonalData]
        [Display(Name = "OtherAddress")]
        public string State { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "ZipCode")]
        public string ZipCode { get; set; }

        [PersonalData]
        [Display(Name = "Backup email")]
        public string BackupEmail { get; set; }

        [PersonalData]
        [Display(Name = "Backup phone number")]
        public string BackupPhoneNumber { get; set; }

        [PersonalData]
        [Display(Name = "Authorize")]
        public bool IsAuthorized { get; set; }

        [PersonalData]
        [Display(Name = "Authorize")]
        public bool IsDisabled { get; set; }
    }
}

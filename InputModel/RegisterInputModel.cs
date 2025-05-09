﻿#nullable disable

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.InputModel
{
    public class RegisterInputModel
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
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

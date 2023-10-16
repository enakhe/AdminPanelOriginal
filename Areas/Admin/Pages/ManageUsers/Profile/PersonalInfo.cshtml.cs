#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Interface;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AdminPanel.Areas.Admin.Pages.ManageUsers.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class PersonalInfoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IAuditLog _auditLog;


        public PersonalInfoModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IAuditLog auditLog)
        {
            _userManager = userManager;
            _db = db;
            _auditLog = auditLog;
        }

        [BindProperty]
        public UserPersonalInfoInputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationUser UserData { get; set; }
        public ContactInfo UserContactInfo { get; set; }
        public byte[] UserProfilePicture { get; set; }

        public void LoadAsync(ApplicationUser user)
        {
            ContactInfo userContactInfo = _db.ContactInfos.FirstOrDefault(contactInfo => contactInfo.UserId == user.Id);

            Input = new UserPersonalInfoInputModel()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DOB = user.DOB,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = userContactInfo.Street,
                City = userContactInfo.City,
                State = userContactInfo.State,
                ZipCode = userContactInfo.ZipCode
            };

            UserData = user;
            UserProfilePicture = user.ProfilePicture;
            UserContactInfo = userContactInfo;
        }

        public async Task OnGetAsync(string id)
        {
            _ = ReturnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    LoadAsync(user);
                }
            }
            else
            {
                StatusMessage = "Error, something unexpected happened";
            }
        }

        public async Task<IActionResult> OnPostAsync(UserPersonalInfoInputModel Input, string id, string returnUrl = null)
        {
            returnUrl = ReturnUrl;
            var admin = await _db.Users.FirstOrDefaultAsync(user => user.UserName == "SuperAdmin");

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                StatusMessage = "Error, something unexpected happened";
                LoadAsync(user);
                return Page();
            }

            if (user != null)
            {
                LoadAsync(user);
                var firstName = user.FirstName;
                var lastName = user.LastName;
                var dob = user.DOB;
                var gender = user.Gender;
                var email = user.Email;
                var phoneNumber = user.PhoneNumber;

                ContactInfo contactInfo = await _db.ContactInfos.FirstOrDefaultAsync(contactInfo => contactInfo.UserId == user.Id);

                var streetAddress = contactInfo.Street;
                var cityAddress = contactInfo.City;
                var stateAddress = contactInfo.State;
                var zipCode = contactInfo.ZipCode;

                UserBackUpInfo userBackUpInfo = await _db.UserBackUpInfos.FirstOrDefaultAsync(backup => backup.UserId == user.Id);

                if (Input.FirstName != firstName)
                {
                    user.FirstName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper());
                    await _userManager.UpdateAsync(user);
                }

                if (Input.LastName != lastName)
                {
                    user.LastName = Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());
                    await _userManager.UpdateAsync(user);
                }

                user.FullName = Regex.Replace(Input.FirstName, "^[a-z]", c => c.Value.ToUpper()) + " " + Regex.Replace(Input.LastName, "^[a-z]", c => c.Value.ToUpper());

                if (Input.DOB != dob)
                {
                    user.DOB = Input.DOB;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.Gender != gender)
                {
                    user.Gender = Input.Gender;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.Email != email)
                {
                    IdentityResult setEmail = await _userManager.SetEmailAsync(user, Input.Email);
                    if (!setEmail.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set email address.";
                        await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                        return RedirectToPage();
                    }
                }

                if (Input.PhoneNumber != phoneNumber)
                {
                    IdentityResult setPhoneNumber = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneNumber.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number";
                        await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                        return RedirectToPage();
                    }
                    _ = await _userManager.UpdateAsync(user);
                }

                if (Input.ProfilePicture != null)
                {
                    using (var dataStream = new MemoryStream())
                    {
                        await Input.ProfilePicture.CopyToAsync(dataStream);
                        if (dataStream.Length < 2097152)
                        {
                            user.ProfilePicture = dataStream.ToArray();
                            await _userManager.UpdateAsync(user);
                        }
                        else
                        {
                            StatusMessage = "The file is too large";
                            await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);
                        }
                    }
                }

                if (Input.Street != streetAddress)
                {
                    contactInfo.Street = Input.Street;
                    _db.Entry(contactInfo).State = EntityState.Modified;
                }

                if (Input.City != cityAddress)
                {
                    contactInfo.City = Input.City;
                    _db.Entry(contactInfo).State = EntityState.Modified;
                }

                if (Input.State != stateAddress)
                {
                    contactInfo.State = Input.State;
                    _db.Entry(contactInfo).State = EntityState.Modified;
                }

                if (Input.ZipCode != zipCode)
                {
                    contactInfo.ZipCode = Input.ZipCode;
                    _db.Entry(contactInfo).State = EntityState.Modified;
                }

                _ = await _userManager.UpdateAsync(user);
                _ = await _db.SaveChangesAsync();
                StatusMessage = "User profile has been updated";
                await _auditLog.AddAudit(HttpContext, admin, user, StatusMessage);

                LoadAsync(user);
                return Page();
            }
            return Page();
        }
    }
}

#nullable disable

using AdminPanel.Data;
using AdminPanel.InputModel;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Areas.Admin.Pages.ManageUsers.Profile
{
    [Authorize(Roles = "SuperAdmin")]
    public class AuditLoggingModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public AuditLoggingModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [BindProperty]
        public UserPersonalInfoInputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public ApplicationUser UserData { get; set; }
        public ContactInfo UserContactInfo { get; set; }
        public List<AuditLogging> AuditLoggings { get; set; }
        public List<IGrouping<DateTime, AuditLogging>> AuditGroupLoggings { get; set; }

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

                    AuditLoggings = await _db.AuditLoggings
                        .Where(audit => audit.UserId == user.Id)
                        .Include(audit => audit.AuditDeviceInfo)
                        .OrderByDescending(audit => audit.DateCreated)
                        .ToListAsync();

                    AuditGroupLoggings = AuditLoggings
                        .GroupBy(audit => audit.DateCreated.Date)
                        .ToList();
                }
            }
            else
            {
                StatusMessage = "Error, something unexpected happened";
            }
        }
    }
}

#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace AdminPanel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public UserApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // POST api/<UserApiController>
        [HttpPost]
        [Route("username")]
        public async Task<JsonResult> Post(string username)
        {
            if (username != null)
            {
                var regexItem = new Regex("^[`!@#$%&.^()-+=/><_,|*]*$");

                if (username.Length < 7 || username == null || username == "")
                {
                    return new JsonResult($"Error, '{username}' is to short");
                }

                foreach (char c in username)
                {
                    if (regexItem.IsMatch(c.ToString()))
                    {
                        return new JsonResult($"Error, username '{username}' is invalid, can only contain letters or digits.");
                    }
                }

                var user = await _userManager.FindByNameAsync(username);

                if (user != null)
                {
                    return new JsonResult($"Error, a user with '{username}' already exits");
                }

                return new JsonResult($"'{username}' is valid");
            }
            return new JsonResult($"Error, '{username}' is not valid");
        }
    }
}

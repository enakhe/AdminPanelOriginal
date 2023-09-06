#nullable disable

using AdminPanel.Data;
using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Areas.Admin.Controllers
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
                if (username.Length < 6)
                {
                    return new JsonResult("Error, username is to short");
                }
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    return new JsonResult("Error, a user with that username already exits");
                }
                return new JsonResult("Username is valid");
            }
            return new JsonResult("");
        }
    }
}

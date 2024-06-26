using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _20211129078_butunleme_ozanuysal.Controllers
{
    [Route("api/User/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet("GetAllUsers")]
        public IQueryable<ApplicationUser> List()
        {
            return _userManager.Users;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(string userName, string email, string password)
        {
            if (userName == null || email == null || password == null)
            {
                return BadRequest("Invalid user data.");
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                //FullName = userName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Ok("User created successfully.");
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoAlbum.Shared.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PhotoAlbum.Server.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _dbcontext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UserController(DatabaseContext dbcontext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, JwtTokenGenerator jwtTokenGenerator)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<IdentityResult>> Register([FromBody] RegisterDTO user)
        {
            var res = await _userManager.CreateAsync(new IdentityUser() { UserName = user.Username}, user.Password);
            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO userdto)
        {
            var user = _dbcontext.Users.SingleOrDefault(x => x.UserName == userdto.Username);
            if (user == null)
            {
                return BadRequest("Wrong username");
            }
            var res = await _signInManager.CheckPasswordSignInAsync(user, userdto.Password, false);
            if (res == null)
                return BadRequest("Wrong username or password");
            return Ok(_jwtTokenGenerator.GenerateToken(user));
        }
    }
}

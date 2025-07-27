using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Data;
using GamblersHell.Shared;
using GamblersHell.Shared.Interface;


namespace GamblersHell.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration _configuration;
        IAuthRepository _authRepository;

        public AuthController(IConfiguration configuration, IAuthRepository authRepository)
        {
            _configuration = configuration;
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model, [FromQuery] string? returnUrl)
        {
            var user = await _authRepository.GetUserAuthAsync(model);
            if (user is null)
                return BadRequest();

            var claims = new List<Claim>()
            {
                new Claim(nameof(UserDTO.ID),user.ID.ToString()),
                new Claim(nameof(UserDTO.Username),user.Username),
                new Claim(nameof(UserDTO.Email), user.Email),
                new Claim(nameof(UserDTO.UserVerified), user.UserVerified.ToString())
            };

            var identity = new ClaimsIdentity(claims, GamblersHellConstants.GamblersHellCookieName);
            var principal = new ClaimsPrincipal(identity);

            //Results.SignIn(principal);
            await HttpContext.SignInAsync(principal);
            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string userId)
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        [HttpGet("userinfo")]
        public IActionResult GetUserClaims()
        {
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var json = JsonSerializer.Serialize(roles);
            var claims = User.Claims.Where(c => c.Type != ClaimTypes.Role).ToDictionary(c => c.Type, c => c.Value);
            claims.Add(ClaimTypes.Role, json);
            return Ok(new UserClaimsDTO
            {
                Claims = claims
            });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using GamblersHell.Server.Services;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authorization;

namespace UserController
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ID");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return -1; 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _userService.GetUserByID(id);

            int currentUserId = GetCurrentUserId();
            if (currentUserId != id)
            {
                return Forbid("You can only see on your own account");
            }

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Return the user as a response
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> RegisterUser([FromBody] UserDTO userDTO)
        {
            var registrationSuccess = await _userService.RegisterUser(userDTO);

            if (registrationSuccess)
            {
                return Ok(userDTO);
            }

            // Return a bad request response if the username or email is taken
            return BadRequest(new { message = "Registration failed," });
        }

        [AllowAnonymous]
        [HttpGet("GetTopFiveRulers")]
        public async Task<ActionResult<List<TopFiveRulersDTO>>> GetTopFiveRulers()
        {
            var list = await _userService.GetTopFiveRulers();

            if (list.Count == 0)
            {
                // If no users are found, return a NotFound response
                return NotFound(new { message = "Users not found" });
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("GetTopFiveGentlemen")]
        public async Task<ActionResult<List<TopFiveGentlemenDTO>>> GetTopFiveGentlemen()
        {
            var list = await _userService.GetTopFiveGentlemen();

            if (list.Count == 0)
            {
                return NotFound(new { message = "Users not found" });
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("GetTopFifteenRulers")]
        public async Task<ActionResult<List<TopFifteenRulersDTO>>> GetTopFifteenRulers()
        {
            var list = await _userService.GetTopFifteenRulers();

            if (list.Count == 0)
            {
                return NotFound(new { message = "Users not found" });
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("GetUltimateRuler")]
        public async Task<ActionResult<TopFifteenRulersDTO>> GetUltimateRuler()
        {
            var ruler = await _userService.UltimateRuler();

            if (ruler == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(ruler);
        }

        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] UserChangePasswordDTO model)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != id)
            {
                return Forbid("You can only see on your own account");
            }

            bool result = await _userService.UserChangePassword(id, model.currentPassword, model.newPassword);

            if (result)
            {
                return Ok(new { message = "Password changed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to change password. Please check your current password." });
            }
        }

        [AllowAnonymous]
        [HttpPatch("RequestTokenForgottenPassword")]
        public async Task<IActionResult> RequestTokenForgottenPassword(string email)
        {
            bool result = await _userService.RequestTokenForgottenPassword(email);

            if (result)
            {
                return Ok(new { message = "Token sent" });
            }
            else
            {
                return BadRequest(new { message = "Failed to send token." });
            }
        }

        [AllowAnonymous]
        [HttpPatch("ChangeForgottenPassword")]
        public async Task<IActionResult> ChangeForgottenPassword(string token, string newPassword)
        {

            bool result = await _userService.ChangeForgottenPassword(token, newPassword);

            if (result)
            {
                return Ok(new { message = "Password changed successfully!" });
            }
            else
            {
                return BadRequest(new { message = "Failed to change password." });
            }
        }


        [AllowAnonymous]
        [HttpPatch("RequestVerificationToken")]
        public async Task<IActionResult> RequestVerificationToken(string email)
        {
            bool result = await _userService.RequestVerificationToken(email);

            if (result)
            {
                return Ok(new { message = "Token sent" });
            }
            else
            {
                return BadRequest(new { message = "Failed to send token." });
            }
        }


        [HttpPatch("VerifyUser")]
        public async Task<IActionResult> VerifyUser(string token)
        {

            bool result = await _userService.VerifyUser(token);

            if (result)
            {
                return Ok(new { message = "User verified successfully!" });
            }
            else
            {
                return BadRequest(new { message = "Failed to verify user." });
            }
        }
    }
}

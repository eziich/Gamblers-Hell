using GamblersHell.Server.Services;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamblersHell.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // Helper method to get current user ID from claims
        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ID");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return -1; 
        }

        // Helper method to get verification status
        private int GetCurrentUserVerification()
        {
            var userVerifyClaim = User.Claims.FirstOrDefault(c => c.Type == "UserVerified");
            if (userVerifyClaim != null && userVerifyClaim.Value == 0.ToString())
            {
                return 0;
            }
            return 1;
        }

        [HttpPatch("GameWonTransaction")]
        public async Task<IActionResult> GameWonTransaction(int id, int priceValue, int level, string gameType, string sessionToken)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0) {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.GameWonTransaction(id, priceValue, level, gameType, sessionToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateGameSession")]
        public async Task<IActionResult> CreateGameSession(string gameType)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId == -1)
                {
                    return Unauthorized("User not authenticated");
                }

                var sessionToken = await _transactionService.CreateGameSession(currentUserId, gameType);
                if (string.IsNullOrEmpty(sessionToken))
                {
                    return BadRequest("Failed to create game session");
                }

                return Ok(sessionToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("GameLostBetTransaction")]
        public async Task<IActionResult> GameLostBetTransaction(int id, int priceValue)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.GameLostBetTransaction(id, priceValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("BeastRaceTheEye")]
        public async Task<IActionResult> BeastRaceTheEye(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.BeastRaceTheEye(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("ClaimTheEye")]
        public async Task<IActionResult> ClaimTheEye(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.ClaimTheEye(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("PokerGameControlReset")]
        public async Task<IActionResult> PokerGameControl(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.PokerGameControlReset(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("PokerGameControlWon")]
        public async Task<IActionResult> PokerGameControlWon(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.PokerGameControlWon(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("PokerGameControlLost")]
        public async Task<IActionResult> PokerGameControlLost(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.PokerGameControlLost(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("PandoraGameControlReset")]
        public async Task<IActionResult> PandoraGameControlReset(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.PandoraGameControlReset(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("PandoraGameControlWon")]
        public async Task<IActionResult> PandoraGameControlWon(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.PandoraGameControlWon(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("PandoraGameControlLost")]
        public async Task<IActionResult> PandoraGameControlLost(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.PandoraGameControlLost(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("LostToBaal")]
        public async Task<IActionResult> LostToBaal(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.LostToBaal(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("LostToLilim")]
        public async Task<IActionResult> LostToLilim(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.LostToLilim(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("LostToSatan")]
        public async Task<IActionResult> LostToSatan(int id)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid("You can only perform transactions on your own account");
                }

                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var result = await _transactionService.LostToSatan(id);
                await HttpContext.SignOutAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
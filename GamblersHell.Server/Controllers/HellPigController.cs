using GamblersHell.Server.Services;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamblersHell.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HellPigController : Controller
    {
        private readonly HellPigService _hellPigService;

        public HellPigController(HellPigService hellPigService)
        {
            _hellPigService = hellPigService;
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

        private int GetCurrentUserVerification()
        {
            var userVerifyClaim = User.Claims.FirstOrDefault(c => c.Type == "UserVerified");
            if (userVerifyClaim != null && userVerifyClaim.Value == 0.ToString())
            {
                return 0;
            }
            return 1;
        }

        [HttpGet("Signs")]
        public async Task<IActionResult> GetSlotSigns()
        {
            List<HellPigDTO> slots;

            int verifiedUser = GetCurrentUserVerification();
            if (verifiedUser == 0)
            {
                return Forbid("You need to verify your account for this action");
            }

            try
            {
                slots = await _hellPigService.GetPigSigns();
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Sign")]
        public async Task<IActionResult> GetSlots()
        {
            try
            {
                int verifiedUser = GetCurrentUserVerification();
                if (verifiedUser == 0)
                {
                    return Forbid("You need to verify your account for this action");
                }

                var randomSlots = await _hellPigService.GetRandomPigSlot();
                return Ok(randomSlots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("DailyReward")]
        public async Task<IActionResult> DailyReward(int id)
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

                var result = await _hellPigService.DailyReward(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("DailyRewardPrice")]
        public async Task<IActionResult> DailyRewardPrice(int id, int priceValue)
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

                var result = await _hellPigService.DailyRewardPrice(id, priceValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

using GamblersHell.Server.Services;
using GamblersHell.Services;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamblersHell.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LadyController : Controller
    {
        private readonly LadyService _ladyService;

        public LadyController(LadyService ladyService)
        {
            _ladyService = ladyService;
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> GetLadyCards()
        {
            List<LadyCardsDTO> cards;

            try
            {
                cards = await _ladyService.GetLadyCardsAsync();
                return Ok(cards); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Card")]
        public async Task<IActionResult> GetRandomCard()
        {
            try
            {
                var randomCard = await _ladyService.GetRandomCardAsync();
                return Ok(randomCard); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

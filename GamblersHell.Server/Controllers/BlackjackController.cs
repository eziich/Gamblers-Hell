using Microsoft.AspNetCore.Mvc;
using GamblersHell.Services;
using GamblersHell.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GamblersHell.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlackjackController : Controller
    {
        private readonly BlackjackService _blackJackService;

        public BlackjackController(BlackjackService blackJackService)
        {
            _blackJackService = blackJackService;
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> GetBlackJackKarte()
        {
            List<BlackjackCardsDTO> cards;

            try
            {
                cards = await _blackJackService.GetBlackjackCards();
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
                var randomCard = await _blackJackService.GetRandomCardAsync();
                return Ok(randomCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

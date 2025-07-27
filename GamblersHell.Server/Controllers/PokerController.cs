using Microsoft.AspNetCore.Mvc;
using GamblersHell.Services;
using GamblersHell.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using GamblersHell.Server.Services;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authorization;

namespace GamblersHell.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PokerController : Controller
    {
        private readonly PokerService _pokerService;

        public PokerController(PokerService pokerService)
        {
            _pokerService = pokerService;
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> GetBlackJackKarte()
        {
            List<PokerCardsDTO> cards;

            try
            {
                cards = await _pokerService.GetPokerCards();
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
                var randomCard = await _pokerService.GetRandomCardAsync();
                return Ok(randomCard); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using GamblersHell.Services;
using GamblersHell.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authorization;

namespace GamblersHell.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CardWarsController : Controller
    {
        private readonly CardWarsService _cardWarsService;

        public CardWarsController(CardWarsService cardWarsService)
        {
            _cardWarsService = cardWarsService;
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> GetCardWarsCards()
        {
            try
            {
                List<CardWarsCardsDTO> cards = await _cardWarsService.GetCardsAsync();
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
                var randomCard = await _cardWarsService.GetRandomCardAsync();
                return Ok(randomCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

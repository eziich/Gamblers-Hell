using Microsoft.AspNetCore.Mvc;
using GamblersHell.Server.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GamblersHell.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HereticsRouletteController : ControllerBase
    {
        private readonly HereticsRouletteService _hereticsRouletteService;

        public HereticsRouletteController(HereticsRouletteService hereticsRouletteService)
        {
            _hereticsRouletteService = hereticsRouletteService;
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> GetAllCards()
        {
            try
            {
                var cards = await _hereticsRouletteService.GetAllCards();
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
                var randomCard = await _hereticsRouletteService.GetRandomCardAsync();
                return Ok(randomCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

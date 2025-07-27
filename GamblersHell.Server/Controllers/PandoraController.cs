using GamblersHell.Models;
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
    public class PandoraController : Controller
    {
        private readonly PandoraService _pandoraService;

        public PandoraController(PandoraService pandoraService)
        {
            _pandoraService = pandoraService;
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> GetPandoraCards()
        {
            List<PandoraCardsDTO> cards;

            try
            {
                cards = await _pandoraService.GetPandoraCards();
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
                var randomCard = await _pandoraService.GetRandomCardAsync();
                return Ok(randomCard); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

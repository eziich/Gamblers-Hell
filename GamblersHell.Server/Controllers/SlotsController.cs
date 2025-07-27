using Microsoft.AspNetCore.Mvc;
using GamblersHell.Models;
using GamblersHell.Server.Services;
using GamblersHell.Services;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Authorization;

namespace GamblersHell.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SlotsController : Controller
    {
        private readonly SlotsService _slotsService;

        public SlotsController(SlotsService slotsService)
        {
            _slotsService = slotsService;
        }

        [HttpGet("SlotsSigns")]
        public async Task<IActionResult> GetSlotSigns()
        {
            List<SlotsSymbolsDTO> slots;

            try
            {
                slots = await _slotsService.GetSlotSigns();
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SlotsSign")]
        public async Task<IActionResult> GetSlots()
        {
            try
            {
                var randomSlots = await _slotsService.GetRandomSlot();
                return Ok(randomSlots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
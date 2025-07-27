using Microsoft.AspNetCore.Mvc;
using GamblersHell.Services;
using GamblersHell.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using GamblersHell.Shared;
using GamblersHell.Server.Services;
using Microsoft.AspNetCore.Authorization;

namespace GamblersHell.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RiddleController : Controller
    {
        private readonly RiddleService _riddleService;

        public RiddleController(RiddleService riddleService)
        {
            _riddleService = riddleService;
        }

        [HttpGet("Symbols")]
        public async Task<IActionResult> GetSymbols()
        {
            try
            {
                List<RiddleSymbolsDTO> symbols = await _riddleService.GetSymbols();
                return Ok(symbols);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Symbol")]
        public async Task<IActionResult> GetRandomSymbol()
        {
            try
            {
                var randomSymbol = await _riddleService.GetRandomSymbol();
                return Ok(randomSymbol);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

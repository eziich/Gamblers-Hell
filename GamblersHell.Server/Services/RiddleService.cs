using System.Text.Json;
using GamblersHell.Shared;

namespace GamblersHell.Services
{
    public class RiddleService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RiddleService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<RiddleSymbolsDTO>> GetSymbols()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "RiddleSymbols.json");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var symbols = await JsonSerializer.DeserializeAsync<List<RiddleSymbolsDTO>>(fileStream);

            return symbols;
        }

        public async Task<RiddleSymbolsDTO> GetRandomSymbol()
        {
            var symbol = await GetSymbols();
            var random = new Random();
            int randomIndex = random.Next(0, symbol.Count);
            return symbol[randomIndex];
        }
    }
}



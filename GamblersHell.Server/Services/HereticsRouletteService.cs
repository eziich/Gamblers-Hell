using System.Text.Json;
using GamblersHell.Shared;

namespace GamblersHell.Server.Services
{
    public class HereticsRouletteService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HereticsRouletteService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<HereticsRouletteCardsDTO>> GetAllCards()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "HereticsRouletteCards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var cards = await JsonSerializer.DeserializeAsync<List<HereticsRouletteCardsDTO>>(fileStream);

            return cards;
        }


        public async Task<HereticsRouletteCardsDTO> GetRandomCardAsync()
        {
            var cards = await GetAllCards();
            var random = new Random();
            int randomIndex = random.Next(0, cards.Count);

            return cards[randomIndex];
        }
    }
}

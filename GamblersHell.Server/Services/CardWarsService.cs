using System.Text.Json;
using GamblersHell.Shared;

namespace GamblersHell.Services
{
    public class CardWarsService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CardWarsService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<CardWarsCardsDTO>> GetCardsAsync()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "CardWarsCards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var cards = await JsonSerializer.DeserializeAsync<List<CardWarsCardsDTO>>(fileStream);

            return cards;
        }

        public async Task<CardWarsCardsDTO> GetRandomCardAsync()
        {
            var cards = await GetCardsAsync();
            var random = new Random();
            int randomIndex = random.Next(0, cards.Count);

            return cards[randomIndex];
        }
    }
}



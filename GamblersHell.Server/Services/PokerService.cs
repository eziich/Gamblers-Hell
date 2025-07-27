using System.Text.Json;
using GamblersHell.Shared;

namespace GamblersHell.Services
{
    public class PokerService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PokerService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<PokerCardsDTO>> GetPokerCards()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "PokerCards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var cards = await JsonSerializer.DeserializeAsync<List<PokerCardsDTO>>(fileStream);

            return cards;
        }

        public async Task<PokerCardsDTO> GetRandomCardAsync()
        {
            var cards = await GetPokerCards();
            var random = new Random();
            int randomIndex = random.Next(0, cards.Count); 
            return cards[randomIndex]; 
        }
    }
}


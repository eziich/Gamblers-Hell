using GamblersHell.Models;
using System.Text.Json;

namespace GamblersHell.Services
{
    public class BlackjackService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlackjackService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<BlackjackCardsDTO>> GetBlackjackCards()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "BlackjackCards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var cards = await JsonSerializer.DeserializeAsync<List<BlackjackCardsDTO>>(fileStream);

            return cards;
        }

        public async Task<BlackjackCardsDTO> GetRandomCardAsync()
        {
            var cards = await GetBlackjackCards();
            var random = new Random();
            int randomIndex = random.Next(0, cards.Count); 

            return cards[randomIndex];
        }
    }
}

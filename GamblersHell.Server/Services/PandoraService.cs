using GamblersHell.Shared;
using System.Text.Json;

namespace GamblersHell.Server.Services
{
    public class PandoraService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PandoraService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<PandoraCardsDTO>> GetPandoraCards()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "PandoraCards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var cards = await JsonSerializer.DeserializeAsync<List<PandoraCardsDTO>>(fileStream);

            return cards;
        }

        public async Task<PandoraCardsDTO> GetRandomCardAsync()
        {
            var cards = await GetPandoraCards();
            var random = new Random();
            int randomIndex = random.Next(0, cards.Count);
            return cards[randomIndex];
        }
    }
}


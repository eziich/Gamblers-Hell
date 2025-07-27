using GamblersHell.Shared;
using System.Text.Json;

namespace GamblersHell.Server.Services
{
    public class LadyService
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public LadyService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<LadyCardsDTO>> GetLadyCardsAsync()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "LadyCards.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var cards = await JsonSerializer.DeserializeAsync<List<LadyCardsDTO>>(fileStream);

            return cards;
        }


        public async Task<LadyCardsDTO> GetRandomCardAsync()
        {
            var cards = await GetLadyCardsAsync();
            var random = new Random();
            int randomIndex = random.Next(0, cards.Count);
            return cards[randomIndex]; 
        }
    }
}

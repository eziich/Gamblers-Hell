using System.Text.Json;
using GamblersHell.Shared;

namespace GamblersHell.Server.Services
{
    public class SlotsService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SlotsService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<SlotsSymbolsDTO>> GetSlotSigns()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "CardsJsons", "SlotsSymbols.json");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var slots = await JsonSerializer.DeserializeAsync<List<SlotsSymbolsDTO>>(fileStream);
            return slots;
        }

        public async Task<List<SlotsSymbolsDTO>> GetRandomSlot()
        {
            var slots = await GetSlotSigns();
            var random = new Random();
            var randomSlots = new List<SlotsSymbolsDTO>();

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = random.Next(0, slots.Count);
                randomSlots.Add(slots[randomIndex]);
            }

            return randomSlots;
        }
    }
}

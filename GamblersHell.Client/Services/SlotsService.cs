using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class SlotsService
    {
        private readonly HttpClient _httpClient;

        public SlotsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<SlotsSymbolsDTO>> GetSlotsSignsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<SlotsSymbolsDTO>>("api/Slots/SlotsSigns");
            return response ?? new List<SlotsSymbolsDTO>(); 
        }

        public async Task<SlotsSymbolsDTO> GetSlotsSignAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<SlotsSymbolsDTO>("api/Slots/SlotsSign");
            return response;
        }
    }
}

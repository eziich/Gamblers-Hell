using GamblersHell.Shared;
using System.Net.Http.Json;

namespace GamblersHell.Client.Services
{
    public class HereticsRouletteService
    {
        private readonly HttpClient _httpClient;

        public HereticsRouletteService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<HereticsRouletteCardsDTO>> GetHereticsRouletteCardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<HereticsRouletteCardsDTO>>("api/HereticsRoulette/Cards");
            return response ?? new List<HereticsRouletteCardsDTO>();
        }

        public async Task<HereticsRouletteCardsDTO> GetRandomCardAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<HereticsRouletteCardsDTO>("api/HereticsRouletteModel/Card");
            return response;
        }
    }
}

using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class PokerService
    {
        private readonly HttpClient _httpClient;

        public PokerService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<PokerCardsDTO>> GetPokerCardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<PokerCardsDTO>>("api/Poker/Cards");
            return response ?? new List<PokerCardsDTO>(); 
        }

        public async Task<PokerCardsDTO> GetRandomCardAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<PokerCardsDTO>("api/Poker/Card");
            return response;
        }
    }
}

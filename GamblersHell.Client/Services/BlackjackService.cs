using GamblersHell.Models;
using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class BlackjackService
    {
        private readonly HttpClient _httpClient;

        public BlackjackService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<BlackjackCardsDTO>> GetBlackjackCardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlackjackCardsDTO>>("api/Blackjack/Cards");
            return response ?? new List<BlackjackCardsDTO>(); 
        }

        public async Task<BlackjackCardsDTO> GetRandomCardAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<BlackjackCardsDTO>("api/Blackjack/Card");
            return response;
        }
    }
}

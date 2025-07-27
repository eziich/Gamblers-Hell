using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class CardWarsService
    {
        private readonly HttpClient _httpClient;

        public CardWarsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<CardWarsCardsDTO>> GetCardWarsModelsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<CardWarsCardsDTO>>("api/CardWars/Cards");
            return response ?? new List<CardWarsCardsDTO>(); 
        }

        public async Task<CardWarsCardsDTO> GetRandomCardAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CardWarsCardsDTO>("api/CardWars/Card");
            return response;
        }
    }
}

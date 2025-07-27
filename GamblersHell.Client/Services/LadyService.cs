using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class LadyService
    {
        private readonly HttpClient _httpClient;

        public LadyService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<LadyCardsDTO>> GetLadyCardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<LadyCardsDTO>>("api/Lady/Cards");
            return response ?? new List<LadyCardsDTO>(); 
        }

        public async Task<LadyCardsDTO> GetRandomCardAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<LadyCardsDTO>("api/Lady/Card");
            return response;
        }
    }
}

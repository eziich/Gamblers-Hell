using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class PandoraService
    {
        private readonly HttpClient _httpClient;

        public PandoraService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<PandoraCardsDTO>> GetPandoraCardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<PandoraCardsDTO>>("api/Pandora/Cards");
            return response ?? new List<PandoraCardsDTO>(); 
        }

        public async Task<PandoraCardsDTO> GetRandomCardAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<PandoraCardsDTO>("api/Pandora/Card");
            return response;
        }
    }
}

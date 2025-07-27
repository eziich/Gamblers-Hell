using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class RiddleService
    {
        private readonly HttpClient _httpClient;

        public RiddleService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<List<RiddleSymbolsDTO>> GetRiddleSymbolsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<RiddleSymbolsDTO>>("api/Riddle/Symbols");
            return response ?? new List<RiddleSymbolsDTO>();
        }

        public async Task<RiddleSymbolsDTO> GetRiddleSymbolAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<RiddleSymbolsDTO>("api/Riddle/Symbol");
            return response;
        }
    }
}

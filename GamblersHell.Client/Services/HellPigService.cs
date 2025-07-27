using GamblersHell.Shared;
using System.Net.Http.Json;


namespace GamblersHell.Client.Services
{
    public class HellPigService
    {
        private readonly HttpClient _httpClient;

        public HellPigService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<HellPigDTO> GetPigSignAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<HellPigDTO>("api/HellPig/Sign");
            return result;
        }

        public async Task<bool> DailyRewards(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<bool>($"api/HellPig/DailyReward?id={id}");
            return result;
        }

        public async Task<bool> DailyRewardsPrice(int id, int priceValue)
        {
            // Create a request object with the parameters
            var response = await _httpClient.PatchAsync($"api/HellPig/DailyRewardPrice?id={id}&priceValue={priceValue}", null); // You can pass content here if needed

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}

using GamblersHell.Shared;
using GamblersHell.Shared.Services;

namespace GamblersHell.Client.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;

        public TransactionService(System.Net.Http.IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<string> GameWonTransaction(int id, int priceValue, int level, string gameType, string sessionToken)
        {
            try
            {
                Console.WriteLine($"Client: Starting GameWonTransaction with id={id}, priceValue={priceValue}, level={level}, gameType={gameType}");

                // Create URL with query parameters
                string url = $"api/Transaction/GameWonTransaction?id={id}&priceValue={priceValue}&level={level}&gameType={gameType}&sessionToken={sessionToken}";
                Console.WriteLine($"Client: Sending request to URL: {url}");

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, new StringContent("", System.Text.Encoding.UTF8, "application/json"));

                // Log response status
                Console.WriteLine($"Client: Received response with status: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                // Read the response as string directly
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Client: Response content: {result}");

                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Client ERROR: {ex.Message}");
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> GameLostBetTransaction(int id, int priceValue)
        {
            try
            {
                string url = $"api/Transaction/GameLostBetTransaction?id={id}&priceValue={priceValue}";
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> BeastRaceTheEye(int id)
        {
            try
            {
                string url = $"api/Transaction/BeastRaceTheEye?id={id}";
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> ClaimTheEye(int id)
        {
            try
            {
                string url = $"api/Transaction/ClaimTheEye?id={id}";
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> PokerGameControlReset(int id)
        {
            try
            {
                // Create URL with query parameters
                string url = $"api/Transaction/PokerGameControlReset?id={id}";

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> PokerGameControlWon(int id)
        {
            try
            {
                // Create URL with query parameters
                string url = $"api/Transaction/PokerGameControlWon?id={id}";

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> PokerGameControlLost(int id)
        {
            try
            {
                // Create URL with query parameters
                string url = $"api/Transaction/PokerGameControlLost?id={id}";

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> PandoraGameControlReset(int id)
        {
            try
            {
                // Create URL with query parameters
                string url = $"api/Transaction/PandoraGameControlReset?id={id}";

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> PandoraGameControlWon(int id)
        {
            try
            {
                // Create URL with query parameters
                string url = $"api/Transaction/PandoraGameControlWon?id={id}";

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }


        public async Task<string> PandoraGameControlLost(int id)
        {
            try
            {
                string url = $"api/Transaction/PandoraGameControlLost?id={id}";

                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> LostToBaal(int id)
        {
            try
            {
                // Create URL with query parameters
                string url = $"api/Transaction/LostToBaal?id={id}";

                // Send the PATCH request
                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<string> LostToLilim(int id)
        {
            try
            {
                string url = $"api/Transaction/LostToLilim?id={id}";

                var response = await _httpClient.PatchAsync(url, null);
                response.EnsureSuccessStatusCode();

                // Read the response as string first, not direct JSON deserialization
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<bool> LostToSatan(int userId)
        {
            try
            {
                // Use DELETE method with userId as route parameter
                var response = await _httpClient.DeleteAsync($"api/Transaction/LostToSatan?id={userId}");

                if (response.IsSuccessStatusCode)
                {
                    // Optionally read the result if you need it
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return true;
                }

                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"LostToSatan failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error in LostToSatan: {ex.Message}", ex);
            }
        }

        public async Task<string> CreateGameSession(string gameType)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Transaction/CreateGameSession?gameType={gameType}", null);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to create game session: {ex.Message}", ex);
            }
        }
    }
}

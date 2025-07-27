using GamblersHell.Shared;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;



namespace GamblersHell.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(GamblersHellConstants.HttpClientName);
        }

        public async Task<UserDTO> GetUserByID(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<UserDTO>($"api/User/{id}");
            if (response !=null)
            {
                return response;    
            }

            return new UserDTO();
        }

        public async Task<bool> RegisterUserAsync(UserDTO user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User", user);

                if (response.IsSuccessStatusCode)
                {
                    return true; 
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Registration failed: {errorMessage}");
                    return false; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while registering the user: {ex.Message}");
                return false; 
            }
        }

        public async Task<List<TopFiveRulersDTO>> TopFiveRulers()
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<TopFiveRulersDTO>>("api/User/GetTopFiveRulers");
                return list ?? new List<TopFiveRulersDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<TopFiveRulersDTO>();
            }
        }

        public async Task<List<TopFiveGentlemenDTO>> TopFiveGentlemen()
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<TopFiveGentlemenDTO>>("api/User/GetTopFiveGentlemen");
                return list ?? new List<TopFiveGentlemenDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<TopFiveGentlemenDTO>();
            }
        }

        public async Task<List<TopFifteenRulersDTO>> TopFifteenRulers()
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<TopFifteenRulersDTO>>("api/User/GetTopFifteenRulers");
                return list ?? new List<TopFifteenRulersDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<TopFifteenRulersDTO>();
            }
        }

        public async Task<TopFifteenRulersDTO> UltimateRuler()
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<TopFifteenRulersDTO>("api/User/GetUltimateRuler");
                return list ?? new TopFifteenRulersDTO ();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new TopFifteenRulersDTO ();
            }
        }

        public async Task<bool> UserChangePassword(int id, string currentPassword, string newPassword)
        {
            try
            {
                // Create the request content with current and new passwords
                var changePasswordModel = new UserChangePasswordDTO
                {
                    currentPassword = currentPassword,
                    newPassword = newPassword
                };

                // Serialize the model to JSON
                var content = new StringContent(JsonSerializer.Serialize(changePasswordModel),Encoding.UTF8,"application/json");

                // Send the PATCH request to the API
                var changePasswordResponse = await _httpClient.PatchAsync($"api/User/ChangePassword?id={id}", content);

                // Check if the request was successful (status code 200-299)
                return changePasswordResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during password change: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RequestTokenForgottenPassword(string email)
        {
            try
            {
                var requestTokenResponse = await _httpClient.PatchAsync($"api/User/RequestTokenForgottenPassword?email={email}", null);
                return requestTokenResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during password token request: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangeForgottenPassword(string token, string newPassword)
        {
            try
            {
                var requestTokenResponse = await _httpClient.PatchAsync($"api/User/ChangeForgottenPassword?token={token}&newPassword={newPassword}", null);
                return requestTokenResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during password token request: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RequestVerificationToken(string email)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/User/RequestVerificationToken?email={email}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during verification token request: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerifyUser(string token)
        {
            try
            {
                // URL encode the token to handle special characters
                var encodedToken = Uri.EscapeDataString(token);
                var requestTokenResponse = await _httpClient.PatchAsync($"api/User/VerifyUser?token={encodedToken}", null);

                // Check if the request was successful (status code 200-299)
                return requestTokenResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during verification: {ex.Message}");
                return false;
            }
        }
    }
}

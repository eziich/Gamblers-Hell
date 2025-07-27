using GamblersHell.Shared;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace GamblersHell.Client.StateProviders
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        HttpClient _httpClient;
        // Add this field to store the current user
        private ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity());

        public CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiHttpClient");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            user = await GetUserStateAsync();
            return new AuthenticationState(user);
        }

        public async Task<HttpResponseMessage> Login(LoginDTO loginDto)
        {
            var result = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginDto);
            if (result.IsSuccessStatusCode)
            {
                // Trigger state change so client knows the user is logged in
                user = await GetUserStateAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
            return result;
        }

        public async Task<HttpResponseMessage> LogOut(ClaimsPrincipal? user)
        {
            var result = await _httpClient.PostAsJsonAsync("/api/Auth/logout", "");
            result.EnsureSuccessStatusCode();
            // Trigger state change so client knows the user is logged out
            this.user = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(this.user)));
            return result;
        }

        private async Task<ClaimsPrincipal> GetUserStateAsync()
        {
            var user = await _httpClient.GetFromJsonAsync<UserClaimsDTO>("api/Auth/userinfo");
            if (user is not { Claims.Count: > 1 })
                return new ClaimsPrincipal(new ClaimsIdentity());

            var claims = new List<Claim>();
            foreach (var claim in user.Claims)
                claims.Add(new Claim(claim.Key, claim.Value));

            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims,
                    GamblersHellConstants.GamblersHellCookieName
                ));
        }
    }
}
using Poc.MutualAuthWithTpm.ClientService.Models;
using System.Net.Http.Json;

namespace Poc.MutualAuthWithTpm.ClientService.Services
{
    public class AuthenticationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthenticationService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetTokenAsync()
        {
            var httpClient = _httpClientFactory
                    .CreateClient("ProxyHttpClient");

            var httpResponseMessage = await httpClient
                .PostAsync("api/Auth/Login", null);

            httpResponseMessage
                .EnsureSuccessStatusCode();

            var jwtToken = await httpResponseMessage.Content
                .ReadFromJsonAsync<JwtToken>();

            return jwtToken?.Token ?? throw new Exception("Token is empty");
        }
    }
}

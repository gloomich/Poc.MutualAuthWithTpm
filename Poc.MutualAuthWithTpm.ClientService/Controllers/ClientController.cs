using Microsoft.AspNetCore.Mvc;
using Poc.MutualAuthWithTpm.ClientService.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Poc.MutualAuthWithTpm.ClientService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        // GET: api/<TestController>
        [HttpGet]
        public async Task<ActionResult<string>> GetMessageDirectly()
        {
            var httpClient = _httpClientFactory
                    .CreateClient("DirectHttpClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/server/GetMessage");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await GetTokenAsync(httpClient));

            var httpResponseMessage = await httpClient
                .SendAsync(request);

            httpResponseMessage
                .EnsureSuccessStatusCode();

            return await httpResponseMessage.Content
                .ReadAsStringAsync();
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetMessageThroughProxy()
        {
            var httpClient = _httpClientFactory
                    .CreateClient("ProxyHttpClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/server/GetMessage");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await GetTokenAsync(httpClient));

            var httpResponseMessage = await httpClient
                .SendAsync(request);

            httpResponseMessage
                .EnsureSuccessStatusCode();

            return await httpResponseMessage.Content
                .ReadAsStringAsync();
        }

        private async Task<string> GetTokenAsync(HttpClient httpClient)
        {
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

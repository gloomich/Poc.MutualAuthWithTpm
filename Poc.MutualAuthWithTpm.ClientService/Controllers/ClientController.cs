using Microsoft.AspNetCore.Mvc;
using Poc.MutualAuthWithTpm.ClientService.Models;
using Poc.MutualAuthWithTpm.ClientService.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Poc.MutualAuthWithTpm.ClientService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SignalRTestService _signalRTestService;
        private readonly AuthenticationService _authenticationService;

        public ClientController(
            IHttpClientFactory httpClientFactory,
            SignalRTestService signalRTestService,
            AuthenticationService authenticationService)
        {
            _httpClientFactory = httpClientFactory;
            _signalRTestService = signalRTestService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetMessage()
        {
            var httpClient = _httpClientFactory
                    .CreateClient("ProxyHttpClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/server/GetMessage");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await _authenticationService.GetTokenAsync());

            var httpResponseMessage = await httpClient
                .SendAsync(request);

            httpResponseMessage
                .EnsureSuccessStatusCode();

            return await httpResponseMessage.Content
                .ReadAsStringAsync();
        }

        [HttpGet]
        public async Task<ActionResult<string>> SendSignalRMessage(string msg)
        {
            return await _signalRTestService.SendMessageAsync(msg);
        }

        [HttpGet]
        public async Task<ActionResult> BroadcastSignalRMessage(string msg)
        {
            await _signalRTestService.BroadcastMessageAsync(msg);

            return Ok();
        }
    }
}

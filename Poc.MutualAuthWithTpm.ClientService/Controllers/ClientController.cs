using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Ping()
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://localhost:7234/api/server/ping2");

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            return httpResponseMessage.IsSuccessStatusCode
                ? Ok()
                : Problem();
        }
    }
}

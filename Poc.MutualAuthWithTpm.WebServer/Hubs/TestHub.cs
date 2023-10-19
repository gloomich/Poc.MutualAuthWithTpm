using Microsoft.AspNetCore.SignalR;

namespace Poc.MutualAuthWithTpm.WebServer.Hubs
{
    public class TestHub : Hub
    {
        public async Task<string> SendMessage(string message)
        {
            return await Task.FromResult($"Received message: {message}");
        }
    }
}

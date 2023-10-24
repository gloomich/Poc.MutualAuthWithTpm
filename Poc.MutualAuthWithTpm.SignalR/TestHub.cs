using Microsoft.AspNetCore.SignalR;

namespace Poc.MutualAuthWithTpm.SignalR
{
    public class TestHub : Hub
    {
        public async Task<string> SendMessage(string message)
        {
            return await Task.FromResult($"Received message: {message}");
        }

        public async Task BroadcastMessage(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }
    }
}

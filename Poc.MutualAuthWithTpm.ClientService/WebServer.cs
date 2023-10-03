using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Poc.MutualAuthWithTpm.ClientService.Config;

namespace Poc.MutualAuthWithTpm.ClientService
{
    internal sealed class WebServer
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _webServerTask = null!;

        public WebServer()
        {

        }

        public void Start()
        {
            _webServerTask = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseStartup<HttpStartup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddClientService(
                            hostContext.Configuration);
                })
                .Build()
                .RunAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _webServerTask.Wait(1000);
        }
    }
}

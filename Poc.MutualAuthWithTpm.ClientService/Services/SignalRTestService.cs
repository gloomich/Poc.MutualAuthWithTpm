using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Topshelf.Configurators;

namespace Poc.MutualAuthWithTpm.ClientService.Services
{
    public class SignalRTestService
    {
        private readonly HubConnection _connection;
        private readonly AuthenticationService _authenticationService;

        public SignalRTestService(
            AuthenticationService authenticationService,
            IConfiguration config)
        {
            _authenticationService = authenticationService;

            var hubPath = Path.Combine(config.GetValue<string>("AppGateway"), "SignalRTest");
            _connection = new HubConnectionBuilder()
                .WithUrl(hubPath, options =>
                {
                    options.AccessTokenProvider = _authenticationService.GetTokenAsync;
                    options.ClientCertificates.Add(CertAccessor.Certificate);
                    options.WebSocketConfiguration = socketOptions =>
                    {
                        socketOptions.RemoteCertificateValidationCallback =
                            (sender, certificate, chain, errors) =>
                                errors == System.Net.Security.SslPolicyErrors.None;
                    };

                    //options.HttpMessageHandlerFactory = factory =>
                    //{
                    //    return new AuthenticatedHttpClientHandler(authenticationService)
                    //    {
                    //        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    //    };
                    //};
                })
                .WithAutomaticReconnect(new RetryPolicyLoop())
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .Build();

            _connection.On<string>("Receive", OnReceiveMsg);

            _ = _connection.StartAsync();            
        }

        public async Task<string> SendMessageAsync(string msg)
        {
            return await _connection.InvokeAsync<string>("SendMessage", msg);
        }

        internal async Task BroadcastMessageAsync(string msg)
        {
            await _connection.SendAsync("BroadcastMessage", msg);
        }

        private void OnReceiveMsg(string msg)
        {
            Console.WriteLine(msg);
        }

        public class RetryPolicyLoop : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                return TimeSpan.FromSeconds(1);
            }
        }
    }
}

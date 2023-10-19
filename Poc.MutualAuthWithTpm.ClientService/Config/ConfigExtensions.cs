using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Poc.MutualAuthWithTpm.ClientService.Services;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Poc.MutualAuthWithTpm.ClientService.Config
{
    internal static class ConfigExtensions
    {
        public static IServiceCollection AddClientService(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddHttpClient(
                "ProxyHttpClient",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri(config.GetValue<string>("AppGateway"));

                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.Accept, "application/json");
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.UserAgent, "PocMutualAuthWithTpm");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
                    };
                    handler.ClientCertificates.Add(CertAccessor.Certificate);
                    handler.ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => 
                            policyErrors == SslPolicyErrors.None;
                    return handler;
                });

            services.AddSingleton<SignalRTestService>();
            services.AddSingleton<AuthenticationService>();

            return services;
        }
    }
}

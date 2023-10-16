using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
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
            var certStore = new X509Store(StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            var cert = certStore.Certificates
                .First(x => x.FriendlyName == "TPM POC Client" && x.Verify());
            //.Find(X509FindType.FindByThumbprint, "3ec637d6b318d3ed186a982ec85617909d9dae98", true)[0];

            //var cert =
            //    new X509Certificate2("C:/Cert/client_dev_PocMutualAuth.pfx", "1234");

            services.AddHttpClient(
                "DirectHttpClient",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri("https://localhost:7234/");

                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.Accept, "application/json");
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.UserAgent, "PocMutualAuthWithTpm");
                });

            services.AddHttpClient(
                "ProxyHttpClient",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri("https://localhost:7048/");

                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.Accept, "application/json");
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.UserAgent, "PocMutualAuthWithTpm");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        SslProtocols = SslProtocols.Tls13
                    };
                    handler.ClientCertificates.Add(cert);
                    //handler.ServerCertificateCustomValidationCallback =
                    //    (httpRequestMessage, cert, cetChain, policyErrors) => {
                    //        return true;
                    //    };
                    return handler;
                });

            return services;
        }
    }
}

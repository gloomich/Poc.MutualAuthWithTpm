using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Poc.MutualAuthWithTpm.ClientService.Config
{
    internal static class ConfigExtensions
    {
        public static IServiceCollection AddClientService(
            this IServiceCollection services,
            IConfiguration config)
        {
            services
                .AddHttpClient();
            
            services.AddHttpClient("DirectHttpClient", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://localhost:7234/");

                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/json");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.UserAgent, "PocMutualAuthWithTpm");
            });

            services.AddHttpClient("ProxyHttpClient", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://localhost:7048/");

                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/json");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.UserAgent, "PocMutualAuthWithTpm");
            });

            return services;;
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            return services;;
        }
    }
}

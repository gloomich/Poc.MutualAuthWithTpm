using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Poc.MutualAuthWithTpm.ClientService.Config
{
    internal static class CorsConfigExtensions
    {
        public static IServiceCollection AddCorsForAnyOrigin(
            this IServiceCollection services)
        {
            return services.AddCors(o =>
            {
                o.AddDefaultPolicy(policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .ApplyDefaultSettings();
                });
            });
        }

        private static CorsPolicyBuilder ApplyDefaultSettings(
            this CorsPolicyBuilder builder)
        {
            return builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    }
}

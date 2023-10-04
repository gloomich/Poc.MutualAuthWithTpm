namespace Poc.MutualAuthWithTpm.AppGateway.Config
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddReverseProxy(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddReverseProxy()
                .LoadFromConfig(configuration.GetRequiredSection("ReverseProxy"));

            return services;
        }
    }
}

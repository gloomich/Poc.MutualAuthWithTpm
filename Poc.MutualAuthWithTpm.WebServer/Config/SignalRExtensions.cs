using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Poc.MutualAuthWithTpm.WebServer.Options;
using System.Net.Http.Headers;

namespace Poc.MutualAuthWithTpm.WebServer.Config
{
    public static class SignalRExtensions
    {
        public static ISignalRServerBuilder AddSignalRHub<THub>(
            this IServiceCollection services,
            IConfiguration config,
            string signalRConfigSectionName = "SignalR")
            where THub : Hub
        {
            var signalRConfig = config.GetSection(
                signalRConfigSectionName);
            services.Configure<SignalROptions<THub>>(signalRConfig);
            var signalROptions = signalRConfig
                .Get<SignalROptions<THub>>();

            services.AddJwtAuthPassthrough(signalROptions.HubPath);

            return services
                .AddSignalR()
                .AddHubOptions<THub>(options =>
                {
                    options.EnableDetailedErrors = signalROptions.EnableDetailedErrors;
                    //options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
                    //options.HandshakeTimeout = TimeSpan.FromMinutes(1);
                });
        }

        public static IEndpointRouteBuilder MapSignalRHub<THub>(
            this IEndpointRouteBuilder endpoints)
            where THub : Hub
        {
            var signalROptions = endpoints.ServiceProvider
                .GetRequiredService<IOptions<SignalROptions<THub>>>()
                .Value;

            endpoints.MapHub<THub>(
                signalROptions.HubPath,
                options =>
                {
                    options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                });

            return endpoints;
        }

        private static IServiceCollection AddJwtAuthPassthrough(
            this IServiceCollection services,
            string hubPath)
        {
            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure(o =>
                {
                    if (o.Events == null)
                    {
                        o.Events = new JwtBearerEvents();
                    }

                    o.Events.OnMessageReceived = context =>
                    {
                        // Is request for SignalR
                        var path = context.HttpContext.Request.Path;
                        if (!path.StartsWithSegments(hubPath))
                        {
                            return Task.CompletedTask;
                        }

                        string accessToken = default;
                        if (context.Request.Query.ContainsKey("access_token"))
                        {
                            accessToken = context.Request.Query["access_token"];
                        }
                        else if (!string.IsNullOrEmpty(context.Request
                            .Headers[HeaderNames.Authorization]))
                        {
                            var header = AuthenticationHeaderValue
                                .Parse(context.Request
                                    .Headers[HeaderNames.Authorization]);
                            accessToken = header.Parameter;
                        }

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    };
                });

            return services;
        }
    }
}

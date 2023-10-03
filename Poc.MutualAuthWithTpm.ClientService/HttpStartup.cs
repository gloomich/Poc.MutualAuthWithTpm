using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poc.MutualAuthWithTpm.ClientService.Config;

namespace Poc.MutualAuthWithTpm.ClientService
{
    internal sealed class HttpStartup
    {
        public HttpStartup(IWebHostEnvironment env, IConfiguration configuration)
        {
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment HostingEnvironment { get; }

        public IConfiguration Configuration { get; }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
            });
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers();

            services
                .AddCorsForAnyOrigin()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen();
        }
    }
}


using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

namespace Poc.MutualAuthWithTpm.WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // load cert from pfx
            //var certificate = new X509Certificate2(
            //    "C:/Cert/TpmServerTestAuthCert.pfx",
            //    "TpmServer"
            //);

            var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificate = store.Certificates.OfType<X509Certificate2>()
                .First(c => c.FriendlyName == "TPM Server Test-only Authorization Certificate");

            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(System.Net.IPAddress.Loopback, 44321, listenOptions =>
                {
                    var connectionOptions = new HttpsConnectionAdapterOptions
                    {
                        ServerCertificate = certificate
                    };

                    listenOptions.UseHttps(connectionOptions);
                });
            });

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapGet("/ping", () => Results.Ok())
            .WithName("Ping")
            .WithOpenApi();

            app.Run();
        }
    }
}
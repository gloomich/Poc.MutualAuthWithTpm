using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Poc.MutualAuthWithTpm.AppGateway.Config;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddReverseProxy(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {   
                context.Success();

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

//builder.Services
//    .AddAuthorization(options =>
//    {
//        //options.AddPolicy("AuthenticatedUserPolicy", policy =>
//        //    policy.RequireAuthenticatedUser());
//    });

builder.Services.Configure<KestrelServerOptions>(options =>
{
    var certificate = new X509Certificate2(
        "C:/Cert/server_dev_PocMutualAuth.pfx",
        "1234");

    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = certificate;
        httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        //httpsOptions.ClientCertificateValidation =
        //    (certificate, chain, errors) =>
        //    {                
        //        return errors == SslPolicyErrors.None;
        //    };
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

app.Run();

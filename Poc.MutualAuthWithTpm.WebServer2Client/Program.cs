using Microsoft.AspNetCore.SignalR.Client;

var hubConnection = new HubConnectionBuilder()
                         .WithUrl("https://localhost:7235/SignalRTest")
                         .WithAutomaticReconnect(new RetryPolicyLoop())
                         .Build();

hubConnection.On<string>("Receive",
    message =>
    {
        Console.WriteLine($"SignalR Hub Message: {message}");
    });

await hubConnection.StartAsync();

Console.ReadKey();

public class RetryPolicyLoop : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(1);
    }
}
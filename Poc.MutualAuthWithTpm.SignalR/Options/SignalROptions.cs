namespace Poc.MutualAuthWithTpm.SignalR.Options
{
    public class SignalROptions<THub>
    {
        public required string HubPath { get; set; }

        public bool EnableDetailedErrors { get; set; }
        public required string RedisConnectionString { get; set; }
    }
}

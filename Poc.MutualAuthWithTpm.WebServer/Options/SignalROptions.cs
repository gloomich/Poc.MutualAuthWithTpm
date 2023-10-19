namespace Poc.MutualAuthWithTpm.WebServer.Options
{
    public class SignalROptions<THub>
    {
        public required string HubPath { get; set; }

        public bool EnableDetailedErrors { get; set; } = true;
    }
}

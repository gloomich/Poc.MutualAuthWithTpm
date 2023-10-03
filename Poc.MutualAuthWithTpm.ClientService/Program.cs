using Topshelf;

namespace Poc.MutualAuthWithTpm.ClientService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                Directory.SetCurrentDirectory(AppContext.BaseDirectory);

                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(() => new WebServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("WebServer for testing tpm");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Poc.MutualAuthWithTpm.ClientService
{
    public static class CertAccessor
    {
        private static readonly Lazy<X509Certificate2> _certificate =
            new(() => 
            {
                var certStore = new X509Store(StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);

                return certStore.Certificates
                    .First(x => x.FriendlyName == "TPM POC Client" && x.Verify());
                //.Find(X509FindType.FindByThumbprint, "3ec637d6b318d3ed186a982ec85617909d9dae98", true)[0];

                //var cert =
                //    new X509Certificate2("C:/Cert/client_dev_PocMutualAuth.pfx", "1234");
            });

        public static X509Certificate2 Certificate => _certificate.Value;
    }
}

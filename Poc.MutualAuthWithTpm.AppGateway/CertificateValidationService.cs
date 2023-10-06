using System.Security.Cryptography.X509Certificates;

namespace Poc.MutualAuthWithTpm.AppGateway
{
    public class CertificateValidationService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            var expectedCertificate = new X509Certificate2(
                Path.Combine("C:/Cert/TpmServerTestAuthCert.pfx"), "TpmServer");

            return clientCertificate.Thumbprint == expectedCertificate.Thumbprint;
        }
    }
}

$pfxPassword = ConvertTo-SecureString `
    -String "1234" `
    -Force `
    -AsPlainText

$rootCA = New-SelfSignedCertificate `
    -Subject localhost `
    -DnsName localhost `
    -NotAfter (Get-Date).AddYears(20) `
    -FriendlyName "TPM POC Root CA" `
	-CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyUsageProperty All `
	-KeyUsage CertSign, CRLSign, DigitalSignature

Export-PfxCertificate `
    -Cert $rootCA `
    -FilePath "C:/Cert/root_ca_dev_PocMutualAuth.pfx" `
    -Password $pfxPassword

Import-PfxCertificate `
    -Password $pfxPassword `
    -CertStoreLocation Cert:\LocalMachine\Root `
    -FilePath "C:/Cert/root_ca_dev_PocMutualAuth.pfx"

$serverCert = New-SelfSignedCertificate `
    -Subject localhost `
    -DnsName localhost `
	-Signer $rootCA `
    -NotAfter (Get-Date).AddYears(20) `
    -FriendlyName "TPM POC Server" `
	-KeyUsage CertSign, CRLSign, DigitalSignature `
	-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") `
	-CertStoreLocation cert:\CurrentUser\my

Export-PfxCertificate `
    -Cert $serverCert `
    -FilePath "C:/Cert/server_dev_PocMutualAuth.pfx" `
    -Password $pfxPassword

$clientCert = New-SelfSignedCertificate `
    -Subject localhost `
    -DnsName localhost `
	-Signer $rootCA `
    -NotAfter (Get-Date).AddYears(20) `
    -FriendlyName "TPM POC Client" `
	-Provider "Microsoft Platform Crypto Provider" `
	-KeyUsage DigitalSignature `
	-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2") `
	-KeyExportPolicy NonExportable `
	-Type Custom `
	-CertStoreLocation cert:\CurrentUser\my

#Export-PfxCertificate `
#    -Cert $clientCert `
#    -FilePath "C:/Cert/client_dev_PocMutualAuth.pfx" `
#    -Password $pfxPassword	
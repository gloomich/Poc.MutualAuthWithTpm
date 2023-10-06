# Poc.MutualAuthWithTpm

1 Создать корневой центр сертификации

$rootCA = New-SelfSignedCertificate `
    -Subject localhost `
    -DnsName localhost `
    -NotAfter (Get-Date).AddYears(20) `
    -FriendlyName "TPM POC Root CA" `
	-CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyUsageProperty All `
	-KeyUsage CertSign, CRLSign, DigitalSignature

$pfxPassword = ConvertTo-SecureString `
    -String "1234" `
    -Force `
    -AsPlainText

Export-PfxCertificate `
    -Cert $rootCA `
    -FilePath "C:/Cert/root_ca_dev_PocMutualAuth.pfx" `
    -Password $pfxPassword
	
Import-PfxCertificate -Password $pfxPassword  -CertStoreLocation Cert:\LocalMachine\Root -FilePath "C:/Cert/root_ca_dev_PocMutualAuth.pfx";

2 Создать дочерний сертификат из корневого сертификата

$rootCA = (Get-ChildItem -Path cert:\CurrentUser\My\0fa858780108f60935db99107e0152c33b2ab5bf)

$serverCert = New-SelfSignedCertificate `
    -Subject localhost `
    -DnsName localhost `
	-Signer $rootCA `
    -NotAfter (Get-Date).AddYears(20) `
    -FriendlyName "TPM POC Server" `
	-KeyUsage CertSign, CRLSign, DigitalSignature `
	-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") `
	-CertStoreLocation cert:\CurrentUser\my

$pfxPassword = ConvertTo-SecureString `
    -String "1234" `
    -Force `
    -AsPlainText

Export-PfxCertificate `
    -Cert $serverCert `
    -FilePath "C:/Cert/server_dev_PocMutualAuth.pfx" `
    -Password $pfxPassword

3

$clientCert = New-SelfSignedCertificate `
    -Subject localhost `
    -DnsName localhost `
	-Signer $rootCA `
    -NotAfter (Get-Date).AddYears(20) `
    -FriendlyName "TPM POC Client" `
	-KeyUsage DigitalSignature `
	-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2") `
	-Type Custom `
	-CertStoreLocation cert:\CurrentUser\my

$pfxPassword = ConvertTo-SecureString `
    -String "1234" `
    -Force `
    -AsPlainText

Export-PfxCertificate `
    -Cert $clientCert `
    -FilePath "C:/Cert/client_dev_PocMutualAuth.pfx" `
    -Password $pfxPassword
-----

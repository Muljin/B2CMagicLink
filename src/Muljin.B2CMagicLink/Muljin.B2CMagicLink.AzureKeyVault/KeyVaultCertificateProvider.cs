using System;
using Azure.Security.KeyVault.Certificates;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Keys;
using System.Net;

namespace Muljin.B2CMagicLink.AzureKeyVault
{
	internal class KeyVaultCertificateProvider : ICertificateProvider
	{

        private readonly CertificateClient _certificateClient;
        private readonly CryptographyClientFactory _cryptographyClientFactory;
        private readonly KeyClient _keyClient;

        private readonly AzureKeyVaultOptions _keyVaultOptions;

        private readonly Lazy<CertificatesContainer> _vaultCryptoValues;

        public KeyVaultCertificateProvider(AzureKeyVaultOptions keyVaultOptions,
            CertificateClient certificateClient,
            CryptographyClientFactory cryptographyClientFactory,
            KeyClient keyClient)
		{
            _certificateClient = certificateClient;
            _cryptographyClientFactory = cryptographyClientFactory;
            _keyClient = keyClient;
            _keyVaultOptions = keyVaultOptions ?? throw new ArgumentNullException(nameof(keyVaultOptions));
            _vaultCryptoValues = new Lazy<CertificatesContainer>(() => LoadCertificate(), true);
        }

        public X509SigningCredentials SigningCredentials => _vaultCryptoValues.Value.SigningCredentials;

        public async Task<string> SignDataAsync(byte[] data)
        {
            var sigRes = await _vaultCryptoValues.Value.CryptographyClient.SignDataAsync(SigningCredentials.Algorithm, data);
            return Base64UrlEncoder.Encode(sigRes.Signature);
        }

        private CertificatesContainer LoadCertificate()
        {
            var getCertificateResult = _certificateClient.GetCertificate(_keyVaultOptions.CertificateName);

            var keyId = getCertificateResult.Value.KeyId;
            var cryptographyClient = _cryptographyClientFactory.CreateCryptographyClient(keyId);

            var cert = new X509Certificate2(getCertificateResult.Value.Cer);
            var signingCreds = new X509SigningCredentials(cert);

            var result = new CertificatesContainer(cryptographyClient, signingCreds);

            return result;
        }
    }
}


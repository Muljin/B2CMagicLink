using System;
using Azure.Core;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace Muljin.B2CMagicLink.AzureKeyVault
{
	internal class CryptographyClientFactory
	{
        private readonly TokenCredential _azureTokenCredential;

        public CryptographyClientFactory(TokenCredential azureTokenCredential)
        {
            _azureTokenCredential = azureTokenCredential ?? throw new ArgumentNullException(nameof(azureTokenCredential));
        }

        public CryptographyClient CreateCryptographyClient(Uri keyId)
        {
            return new CryptographyClient(keyId, _azureTokenCredential);
        }
    }
}


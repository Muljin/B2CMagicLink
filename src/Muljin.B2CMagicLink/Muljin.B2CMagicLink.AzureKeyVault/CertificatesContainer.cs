using System;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Muljin.B2CMagicLink.AzureKeyVault
{
    internal class CertificatesContainer
    {
        public CertificatesContainer(CryptographyClient cryptographyClient, X509SigningCredentials signingCredentials)
        {
            CryptographyClient = cryptographyClient ?? throw new ArgumentNullException(nameof(cryptographyClient));
            SigningCredentials = signingCredentials ?? throw new ArgumentNullException(nameof(signingCredentials));
        }

        public CryptographyClient CryptographyClient { get; }
        public X509SigningCredentials SigningCredentials { get; }
    }
}


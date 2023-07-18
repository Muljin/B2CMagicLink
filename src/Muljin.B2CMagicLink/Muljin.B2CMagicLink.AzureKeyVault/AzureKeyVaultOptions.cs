using System;
namespace Muljin.B2CMagicLink.AzureKeyVault
{
	public record AzureKeyVaultOptions
	{
        /// <summary>
        /// Name of the certificate to use for signing tokens
        /// </summary>
		public string CertificateName { get; init; } = string.Empty;

        /// <summary>
        /// Fully qualified keyvault uri.
        /// e.g. https://mykeyvault.vault.azure.net/
        /// </summary>
        public string KeyVaultUri { get; init; } = string.Empty;
	}
}


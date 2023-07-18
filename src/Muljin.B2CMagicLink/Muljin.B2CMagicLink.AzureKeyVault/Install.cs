using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muljin.B2CMagicLink.AzureKeyVault;

namespace Muljin.B2CMagicLink;
public static class Install
{
    /// <summary>
    /// Add MagicLink services based on Azure Key Vault stored certificate and key
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="azureTokenCredentials"></param>
    /// <param name="keyVaultOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddB2CMagicLinkKeyVault(this IServiceCollection services,
        IConfiguration configuration,
        TokenCredential? azureTokenCredentials = null,
        AzureKeyVaultOptions? keyVaultOptions = null)
    {
        azureTokenCredentials ??= new DefaultAzureCredential();

        if (keyVaultOptions == null)
        {
            keyVaultOptions = new AzureKeyVaultOptions();
            configuration.GetSection("MagicLink:KeyVault").Bind(keyVaultOptions);
        }
        services.AddSingleton(keyVaultOptions);

        //validate keyvault options
        ArgumentNullException.ThrowIfNullOrEmpty(keyVaultOptions.KeyVaultUri);
        ArgumentNullException.ThrowIfNullOrEmpty(keyVaultOptions.CertificateName);

        services.AddAzureClients(builder =>
        {
            builder.UseCredential(azureTokenCredentials);
            builder.AddCryptographyClient(new Uri(keyVaultOptions.KeyVaultUri));
            builder.AddKeyClient(new Uri(keyVaultOptions.KeyVaultUri));
            builder.AddCertificateClient(new Uri(keyVaultOptions.KeyVaultUri));
        });
        services.AddScoped(s =>
        {
            return new CryptographyClientFactory(azureTokenCredentials);
        });

        return services;
    }
}


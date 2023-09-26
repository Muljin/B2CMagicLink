using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muljin.B2CMagicLink.AzureKeyVault;

namespace Muljin.B2CMagicLink
{
    public static class Install
    {
        public static IServiceCollection AddB2CMagicLink(this IServiceCollection services,
            IConfiguration configuration,
            OidcOptions? oidcOptions = null)
        {

            if (oidcOptions == null)
            {
                oidcOptions = new OidcOptions();
                configuration.GetSection("MagicLink:Oidc").Bind(oidcOptions);
            }

            services.AddSingleton(oidcOptions);

            //main oidc tokens service
            services.AddScoped<IOidcService, OidcService>();

            return services;
        }

    }
}

        
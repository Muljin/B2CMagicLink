using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Muljin.B2CMagicLink;

/// <summary>
/// Main OpenID Connect service for generating tokens and getting configuration
/// </summary>
public interface IOidcService
{
    /// <summary>
    /// Generate and serialize the OIDC metadata
    /// (for use as result for  .well-known/openid-configuration endpoint)
    /// </summary>
    /// <returns></returns>
    public string GetSerializedOidcMetadata();

    public string GetSerializedJwks();

    public Task<string> BuildSerializedIdTokenAsync(string audience, int duration, List<System.Security.Claims.Claim> claims);

    public Task<string> BuildSerializedIdTokenAsync(string audience, int duration, string email); 
}
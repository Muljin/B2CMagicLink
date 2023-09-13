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

    /// <summary>
    /// Get the JWKs
    /// </summary>
    /// <returns></returns>
    public string GetSerializedJwks();

    /// <summary>
    /// Build a serialised JWT token with a list of claims
    /// </summary>
    /// <param name="audience"></param>
    /// <param name="duration"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    public Task<string> BuildSerializedIdTokenAsync(string audience, int duration, List<System.Security.Claims.Claim> claims);

    /// <summary>
    /// Build a serialized JWT token with the user email as a claim
    /// </summary>
    /// <param name="audience"></param>
    /// <param name="duration"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<string> BuildSerializedIdTokenByEmailAsync(string audience, int duration, string email);

    /// <summary>
    /// Build a serialized JWT token with the user object ID as a claim
    /// </summary>
    /// <param name="audience"></param>
    /// <param name="duration"></param>
    /// <param name="objectId"></param>
    /// <returns></returns>
    public Task<string> BuildSerializedIdTokenByObjectIdAsync(string audience, int duration, string objectId);

}
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Muljin.B2CMagicLink.AzureKeyVault
{
	internal class OidcService : IOidcService
	{
        private const string jwkUriEndpoint = "oidc/.well-known/keys";
        private readonly string _jwksUri;

        private readonly ICertificateProvider _certificateProvider;
        private readonly OidcOptions _oidcOptions;


        private readonly Lazy<string> _jwks;
        private readonly Lazy<string> _oidcMetadata;

        public OidcService(ICertificateProvider certificateProvider, OidcOptions oidcOptions)
        {
            _certificateProvider = certificateProvider;
            _oidcOptions = oidcOptions ?? throw new ArgumentNullException(nameof(oidcOptions));

            //setup data loading
            _oidcMetadata = new Lazy<string>(() => GenerateOidcMetadata(), true);
            _jwks = new Lazy<string>(() => GenerateSerializedJwks(), true);
            
            //setup jwks uri
            var jwkSb = new StringBuilder();
            jwkSb.Append(_oidcOptions.BaseUrl);
            if (!_oidcOptions.BaseUrl.EndsWith('/'))
            {
                jwkSb.Append('/');
            }
            jwkSb.Append(jwkUriEndpoint);
            _jwksUri = jwkSb.ToString();
        }

        public async Task<string> BuildSerializedIdTokenAsync(string audience, int duration, string userEmail)
        {
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim("email", userEmail, System.Security.Claims.ClaimValueTypes.String, _oidcOptions.Issuer)
            };

            return await BuildSerializedIdTokenAsync(audience, duration, claims);
        }

        public async Task<string> BuildSerializedIdTokenAsync(string audience, int duration, List<System.Security.Claims.Claim> claims)
        {

            var header = new JwtHeader(_certificateProvider.SigningCredentials);
            var payload = new JwtPayload(
                    _oidcOptions.Issuer,
                    audience,
                    claims,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(duration));

            // Use the intended JWT Token's Header and Payload value as the data for the token's Signature
            var unsignedTokenText = $"{header.Base64UrlEncode()}.{payload.Base64UrlEncode()}";
            var byteData = Encoding.UTF8.GetBytes(unsignedTokenText);

            //get signature for ehader and payload
            var encodedSignature = await _certificateProvider.SignDataAsync(byteData);

            // Assemble the header, payload, and encoded signatures and return
            var result = $"{unsignedTokenText}.{encodedSignature}";
            return result;
        }


        public string GetSerializedOidcMetadata() => _oidcMetadata.Value;

        public string GetSerializedJwks() => _jwks.Value;

        /// <summary>
        /// Generate and serialize the OIDC metadata (to be used as result for  .well-known/openid-configuration endpoint)
        /// </summary>
        /// <returns></returns>
        private string GenerateOidcMetadata()
        {
            var result = new OidcModel
            {
                Issuer = _oidcOptions.Issuer,
                JwksUri = _jwksUri,
                IdTokenSigningAlgValuesSupported = new[] { _certificateProvider.SigningCredentials.Algorithm },
            };
            var serializedResult = JsonSerialize(result);
            return serializedResult;
        }   

        /// <summary>
        /// Generate and serialize the jwks information
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string GenerateSerializedJwks()
        {
            var certificate = _certificateProvider.SigningCredentials.Certificate;

            string certData = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));

            string thumbprint = Base64UrlEncoder.Encode(certificate.GetCertHash());

            // JWK must have the modulus and exponent explicitly defined
            RSA rsa = certificate.GetRSAPublicKey()!;
            if (rsa == null)
            {
                throw new InvalidOperationException("Certificate is not an RSA certificate.");
            }

            var keyParams = rsa.ExportParameters(false);
            var keyModulus = Base64UrlEncoder.Encode(keyParams.Modulus);
            var keyExponent = Base64UrlEncoder.Encode(keyParams.Exponent);

            var keyModel = new JwksKeyModel
            {
                Kid = _certificateProvider.SigningCredentials.Kid,
                Kty = "RSA", 
                Nbf = new DateTimeOffset(certificate.NotBefore).ToUnixTimeSeconds(),
                Use = "sig",
                Alg = _certificateProvider.SigningCredentials.Algorithm,
                X5C = new[] { certData },
                X5T = thumbprint,
                N = keyModulus,
                E = keyExponent
            };

            var result = new JwksModel
            {
                Keys = new[] { keyModel }
            };
            var serializedResult = JsonSerialize(result);
            return serializedResult;
        }


        private string JsonSerialize(object obj)
        {
            return JsonSerializer.Serialize(obj, options: new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
            });
        }
    }
}


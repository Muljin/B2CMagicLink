using System;
namespace Muljin.B2CMagicLink
{
    public record OidcModel
    {
        public required string Issuer { get; init; }

        public required string JwksUri { get; init; }

        public required ICollection<string> IdTokenSigningAlgValuesSupported { get; init; } = new List<string>();
    }
}


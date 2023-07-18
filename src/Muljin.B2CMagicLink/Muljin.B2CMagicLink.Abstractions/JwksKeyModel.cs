using System;
namespace Muljin.B2CMagicLink
{
    /// <summary>
    /// Json Web Key
    /// https://datatracker.ietf.org/doc/html/rfc7517
    /// </summary>
    public record JwksKeyModel
    {
        public string Kid { get; set; } = string.Empty;

        public long Nbf { get; set; }

        public string Use { get; set; } = string.Empty;

        public string Kty { get; set; } = string.Empty;

        public string Alg { get; set; } = string.Empty;

        public ICollection<string> X5C { get; set; } = new List<string>();

        public string X5T { get; set; } = string.Empty;

        public string N { get; set; } = string.Empty;

        public string E { get; set; } = string.Empty;
    }
}


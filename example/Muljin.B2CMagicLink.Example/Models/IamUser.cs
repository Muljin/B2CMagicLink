using System;
namespace Muljin.B2CMagicLink.Example.Models
{
    public record IamUser
    {
        public bool IsLocalAccount { get; init; }

        public required string Issuer { get; init; }

        public required string Subject { get; init; }

        public required string GivenName { get; init; }

        public required string Surname { get; init; }

        public string? Email { get; init; }

        public string? Telephone { get; init; }
    }
}


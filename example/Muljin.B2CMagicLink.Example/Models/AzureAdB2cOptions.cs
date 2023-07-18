using System;
namespace Muljin.B2CMagicLink.Example.Models
{
    public record AzureAdB2cOptions
    {
        public required string ClientId { get; init; }

        public required string ClientSecret { get; init; }

        public required string DomainOrig { get; init; }

        public required string TenantId { get; init; }

        public required string TokenIssuer { get; init; }

    }
}


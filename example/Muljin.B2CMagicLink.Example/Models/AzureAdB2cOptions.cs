using System;
namespace Muljin.B2CMagicLink.Example.Models
{
    public record AzureAdB2cOptions
    {
        public string ClientId { get; init; } = string.Empty;

        public string ClientSecret { get; init; } = string.Empty;

        public string DomainOrig { get; init; } = string.Empty;

        public string MagicLinkFlowUrl { get; init; } = string.Empty;

        public string TenantId { get; init; } = string.Empty;

        public string TokenIssuer { get; init; } = string.Empty;

    }
}


using System;
namespace Muljin.B2CMagicLink
{
	public record OidcOptions
	{
		/// <summary>
		/// Base URL for the server
		/// </summary>
		public string BaseUrl { get; init; } = string.Empty;

		/// <summary>
		/// Issuer for tokens
		/// </summary>
		public string Issuer { get; init; } = string.Empty;
	}
}


using System;
namespace Muljin.B2CMagicLink.Example.Models
{
	public record SendGridOptions
	{
		public string ApiKey { get; init; } = string.Empty;

		public string Username { get; init; } = string.Empty;

		public string Password { get; init; } = string.Empty;
	}
}


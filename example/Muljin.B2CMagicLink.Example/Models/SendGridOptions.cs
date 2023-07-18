using System;
namespace Muljin.B2CMagicLink.Example.Models
{
	public record SendGridOptions
	{
		public string ApiKey { get; init; } = string.Empty;
	}
}


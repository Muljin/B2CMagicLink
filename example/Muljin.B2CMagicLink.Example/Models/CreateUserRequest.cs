using System;
namespace Muljin.B2CMagicLink.Example.Models
{
    public record CreateUserRequest
    {
        public required string GivenName { get; init; }

        public required string Surname { get; init; }

        public required string Email { get; init; }

        public required string Password { get; init; }

        public string? Telephone { get; init; }
    }
}


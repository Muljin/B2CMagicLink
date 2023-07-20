using System;
using Microsoft.Extensions.Options;
using Muljin.B2CMagicLink.Example.Models;

namespace Muljin.B2CMagicLink.Example.Services
{
	public class UsersService
	{
		private readonly AzureAdB2cOptions _azureAdB2cOptions;
		private readonly AzureAdB2cService _b2cService;
		private readonly EmailService _emailService;
		private readonly IOidcService _oidcService;

		public UsersService(IOptions<Models.AzureAdB2cOptions> azureAdB2cOptions,
			AzureAdB2cService b2cService,
			EmailService emailService,
			IOidcService oidcService)
		{
			_azureAdB2cOptions = azureAdB2cOptions.Value;
			_b2cService = b2cService;
			_emailService = emailService;
			_oidcService = oidcService;
		}

		public async Task SignupOrSigninAsync(string email)
		{
			var user = await _b2cService.GetUserAsync(email);
			if(user == null)
			{
				await _b2cService.CreateUserAsync(new Models.CreateUserRequest()
				{
					Email = email,
					GivenName = "Magic",
					Surname = "Link",
					Password = Muljin.Utils.RandomStringGenerator.GenerateRandomPassword(categoryLength: 16)
				});
			}

			var token = await _oidcService.BuildSerializedIdTokenAsync(_azureAdB2cOptions.ClientId, 15, email);
			await _emailService.SendMagicLinkAsync(email, token);
		}
	}
}


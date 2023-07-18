using System;
namespace Muljin.B2CMagicLink.Example.Services
{
	public class UsersService
	{
		private readonly AzureAdB2cService _b2cService;
		private readonly IOidcService _oidcService;

		public UsersService(AzureAdB2cService b2cService, IOidcService oidcService)
		{
			_b2cService = b2cService;
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

			var token = 
		}
	}
}


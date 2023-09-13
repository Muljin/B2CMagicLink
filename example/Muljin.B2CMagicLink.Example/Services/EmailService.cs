using System;
using System.Web;
using Microsoft.Extensions.Options;
using Muljin.B2CMagicLink.Example.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Muljin.B2CMagicLink.Example.Services
{
	public class EmailService
	{
        //private static string magicLinkFlowUrl = "https://Muljin.b2clogin.com/Muljin.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SIGNIN_WITH_OBJECTID&client_id=64257a7a-3240-4021-8860-45af0bbd6734&nonce=defaultNonce&redirect_uri={0}&scope=openid&response_type=code";

        private readonly string magicLinkFlowUrl;
		private readonly SendGridOptions _sendGridOptions;

        public EmailService(IOptions<SendGridOptions> sendGridOptions,
			IOptions<AzureAdB2cOptions> azureAdB2cOptions)
		{
			_sendGridOptions = sendGridOptions.Value ?? throw new ArgumentNullException(nameof(sendGridOptions));
			magicLinkFlowUrl = $"{azureAdB2cOptions.Value.RedirectUrl}";
		}

		public async Task SendMagicLinkAsync(string email, string token)
		{

			Muljin.Utils.Validators.ValidateEmail(email);

			var from = new EmailAddress("info@muljin.com", "Muljin");
			var to = new EmailAddress(email);


			var content = $"To login, goto {magicLinkFlowUrl}?id_token_hint={token}";

			var htmlContent = $"To login, <a href=\"{magicLinkFlowUrl}?id_token_hint={token}\"> click here </a>";

            var msg = MailHelper.CreateSingleEmail(from, to, "Your Muljin Magic Link Example login link", content, htmlContent);


            var client = new SendGridClient(new SendGridClientOptions()
			{
				ApiKey = _sendGridOptions.ApiKey
			});

			await client.SendEmailAsync(msg);
		}
	}
}


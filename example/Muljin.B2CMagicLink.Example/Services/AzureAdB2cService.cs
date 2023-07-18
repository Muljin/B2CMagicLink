using System;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Muljin.B2CMagicLink.Example.Mappers;
using Muljin.B2CMagicLink.Example.Models;

using ADUser = Microsoft.Graph.Models.User;

namespace Muljin.B2CMagicLink.Example.Services
{
    public class AzureAdB2cService
    {
        private readonly TokenCredential _tokenCredential;
        private readonly string _b2cDomain;
        private readonly string _tokenIssuer;

        private const string defaultScopes = "https://graph.microsoft.com/.default";



        public AzureAdB2cService(IOptions<Models.AzureAdB2cOptions> config)
        {
            _b2cDomain = config.Value.DomainOrig;
            _tokenIssuer = config.Value.TokenIssuer;
            _tokenCredential = new ClientSecretCredential(config.Value.TenantId, config.Value.ClientId, config.Value.ClientSecret);
        }

        public async Task<IamUser> CreateUserAsync(CreateUserRequest request)
        {
            var res = await AddGraphUserAsync(request);
            var iamuser = AzureAdB2cUserObjectMapper.Map(res, _b2cDomain);
            return iamuser!;
        }

        public async Task<IamUser?> GetUserAsync(string email)
        {
            var graphClient = new GraphServiceClient(_tokenCredential, new string[] { defaultScopes });
            var filter = $"identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{_b2cDomain}')";

            var users = await graphClient
                                .Users
                                .GetAsync((a) =>
                                {
                                    a.QueryParameters.Filter = filter;
                                });
                                //.Request()
                                //.Filter(filter)
                                //.GetAsync();

            return users?.Value?.Select(u => AzureAdB2cUserObjectMapper.Map(u, _tokenIssuer)).FirstOrDefault();
        }

        private async Task<ADUser> AddGraphUserAsync(CreateUserRequest req)
        {
            var graphClient = new GraphServiceClient(_tokenCredential, new string[] { defaultScopes });

            //setup theuser object
            var _gUser = AzureAdB2cUserObjectMapper.Map(req, _b2cDomain);

            _gUser!.PasswordPolicies = "DisablePasswordExpiration";
            _gUser!.PasswordProfile = new PasswordProfile()
            {
                ForceChangePasswordNextSignInWithMfa = false,
                ForceChangePasswordNextSignIn = false,
                Password = req.Password
            };
            Console.WriteLine("Creating with issuer: " + _b2cDomain);
            var user = await graphClient.Users.PostAsync(_gUser);

            return user;
        }
    }
}


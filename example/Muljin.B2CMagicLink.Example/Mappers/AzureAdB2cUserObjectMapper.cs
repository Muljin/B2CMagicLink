using System;
using Microsoft.Graph.Models;
using Muljin.B2CMagicLink.Example.Models;

namespace Muljin.B2CMagicLink.Example.Mappers
{
    public class AzureAdB2cUserObjectMapper
    {
        private const string PhoneSignInType = "phoneNumber";
        private const string EmailSignInType = "emailAddress";

        internal static IamUser? Map(Microsoft.Graph.Models.User user, string tokenIssuer)
        {
            if (user == null)
                return null;

            return new IamUser()
            {
                IsLocalAccount = user.CreationType?.Equals("LocalAccount", StringComparison.InvariantCultureIgnoreCase) ?? false,
                Issuer = tokenIssuer,
                Subject = user.Id!,
                GivenName = user.GivenName ?? string.Empty,
                Surname = user.Surname ?? string.Empty,
                Telephone = user.Identities?.FirstOrDefault(u => u.SignInType?.Equals(PhoneSignInType) ?? false)?.IssuerAssignedId,
                Email = user.Identities?.FirstOrDefault(u => u.SignInType == EmailSignInType)?.IssuerAssignedId
            };
        }

        internal static Microsoft.Graph.Models.User? Map(CreateUserRequest req, string b2cDomain)
        {
            if (req == null)
                return null;

            string displayName;
            if (String.IsNullOrWhiteSpace(req.GivenName) || String.IsNullOrWhiteSpace(req.Surname))
            {
                displayName = "Magic Link Example User";
            }
            else
            {
                displayName = $"{req.GivenName} {req.Surname}";
            }

            var res = new Microsoft.Graph.Models.User()
            {
                AccountEnabled = true,
                DisplayName = displayName,
                GivenName = req.GivenName,
                Surname = req.Surname,
                PasswordPolicies = "DisablePasswordExpiration"
            };

            //setup identities list
            var identities = new List<ObjectIdentity>();

            //add email
            if (!String.IsNullOrWhiteSpace(req.Email))
            {
                var identity = new ObjectIdentity()
                {
                    SignInType = EmailSignInType,
                    IssuerAssignedId = req.Email,
                    Issuer = b2cDomain
                };
                identities.Add(identity);
            }

            //return results
            res.Identities = identities;
            return res;
        }
    }
}


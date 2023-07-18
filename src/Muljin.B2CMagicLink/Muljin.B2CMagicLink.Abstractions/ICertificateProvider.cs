using System;
using Microsoft.IdentityModel.Tokens;

namespace Muljin.B2CMagicLink
{
	public interface ICertificateProvider
	{
        /// <summary>
        /// The signing credentials
        /// </summary>
        public X509SigningCredentials SigningCredentials { get; }

        /// <summary>
        /// Sign data and return as base64 encoded string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SignDataAsync(byte[] data);
    }
}


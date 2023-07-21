# Muljin Azure AD B2C Magic Link Library

This library allows you to (relatively) easily integrate Magic Links into your Azure AD B2C tenant using an existing or new Asp.net Web API project. This can be deployed in the same project as your primary backend service.

This library has been tested on .net 7.0. It may work with earlier versions, however all referenced nuget packages are based on the latest available versions and will continue to be updated to the latest available versions where applicable. 

Example can be tested at [http://muljinmagiclinkexample.azurewebsites.net/](http://muljinmagiclinkexample.azurewebsites.net/)

### What is a Magic Link?
A magic link is a passwordless form of authentication where by a token can be emailed or messaged to a user and exchanged for an authentication token. This allows for a smoother authentication flow for users where they do not need to set or remember any additional passwords.

### Requirements
This library requires an existing Asp.net project, Azure subscription and an Azure AD B2C tenant. For details on how to create these please check the relevant documents. However if you are just getting started with Azure AD B2C, we recommend you setup using their more common authentication flows, such as email and password or social flows, before attempting to implement magic links.

This project **MUST** be hosted on an externally accessible URL as required by Azure AD B2C in order to access metadata and Jwks.

### Certificate Storage Support
The library currently only supports certificates stored in Azure KeyVault however additional certificate providers such as HashiCorp Vault or locally stored certificates can be added by implementing the ICertificateProvider interface.

## Project setup
The libraries are built to be included in an existing or self-contained Asp.net core project. For ease of management we recommend including it in your primary backend service, though this can be deployed as an entirely indepedent service. 

### Nuget Packages
Add the following packages to your Asp.net project:

- Muljin.B2CMagicLink
- Muljin.B2CMagicLink.AzureKeyVault

### Identity or credentials
***NOTE** The details below are general purpose instructions, please consult the relevant documentations on how to authenticate your backend service against Azure Key Vault.*

Next, ensure your web api service is able to authenticate against Azure resources, specifically for KeyVault. The example project uses an App Registration, client Id and Client Secret which is than given access to Key Vault. However the AzureKeyVault certificate provider can be authenticated by any TokenCredential, including managed identities or Visual Studio Credentials.

To create an App Registration based credential, add the following line to your startup.cs or program.cs depending on your version of asp.net.
```
TokenCredential creds = new ClientSecretCredential(tenantId, clientId, clientSecret);
```

Where tenantId, clientId and clientSecret are the correct values for your Azure environment and app registration.

## Add library Configuration
Once you've configured the credentials, next add the Muljin.B2CMagicLink and Muljin.B2CMagicLink.AzureKeyVault services by using the IServiceCollection extension methods AddB2CMagicLink(IConfiguration) and AddB2CMagicLinkKeyVault(IConfiguration, TokenCredentials = null)

For example, in asp.net 7, add the following 2 lines in program.cs

```
//add magic link
builder.Services.AddB2CMagicLink(builder.Configuration);
builder.Services.AddB2CMagicLinkKeyVault(builder.Configuration, creds);
```

Where `creds` is the Azure token credentials you should have created earlier.

### KeyVault
For securely storing certificates and signing keys, an Azure Key Vault resource is required. Create a new Key Vault in your Azure subscription and make note of the name. 
In Key Vault, create a new certificate, using the RSA algorithm, leaving the rest of the settings as default.
Add the identity for the App Registration you created earlier and give it the role of Key Vault Certificate Reader.

### Appsettings.json
finally, add the following block to your appsettings.json file or equivalent, replacing the relevant values where required:

```
"MagicLink": {
    "KeyVault": {
      "CertificateName": "{{certname}}",
      "KeyVaultUri": "https://{{keyvaultname}}.vault.azure.net/"
    },
    "Oidc": {
      "BaseUrl": "https://api.yourdomain.com/",
      "Issuer": "https://{{tenantname}}.b2clogin.com/{{tenantid}}/v2.0/"
    } 
  }
```

replacing the following values

| name | description | example |
-------|-------------|---------|
| certname | name of the keyvault certificate | myjwkcert |
| keyvaultname | name of the keyvault resource | mykeyvault |
| baseurl | Base  fully qualified url of your API | https://api.mydomain.com/ |
| tenantname | Your Azure AD B2C tenant domain | myb2ctenant |

### Azure AD B2C Identity Experience Framework (IDF)
In order to be able to use IDF in Azure AD B2C, an Azure subscription must be linked to your azure AD B2C tenant. If you have not done so already, please consult the documentation found here [Azure B2C link To Azure Subscription](https://learn.microsoft.com/en-us/answers/questions/58949/azure-b2c-link-to-azure-subscription)

### Create signing and encryption certificates
Goto Policy Keys and create the following 2 policy keys, selecting "Generate" for both.

| name | Key type | Key usage |
|------|----------|------------|
| B2C_1A_TokenEncryptionKeyContainer | RSA | Encryption |
| B2C_1A_TokenSigningKeyContainer | RSA | Signature |

If you'd like to select your own key names, simply replace the above names in the `B2C_1A_SIGNIN_WITH_EMAIL_MAGICLINK.xml` with your own ones.

#### Preparing and uploading custom policies
You will need to upload the custom policies found in the /custompolicies folder of this repository. If you have already uploaded custom policies as available from the IDF sample pack, then you can upload `B2C_1A_SIGNIN_WITH_EMAIL_MAGICLINK.xml`

Note: The custom policies included in this repository are simplified versions of the sample pack, removing references to social providers such as Facebook. Should you be looking to add additional signup flows we recommend downloaded the base custom policies from the sample pack and uploading only the `B2C_1A_SIGNIN_WITH_EMAIL_MAGICLINK.xml` custom policy on top.

In  the custom policy XML files, do a find and replace all for the following strings

| name | description | example |
-------|-------------|---------|
| {{tenantid}} | Replace with your tenant id. This is the Tenant ID and NOT the subscription Id | 14182de3-6b9b-4138-a0fa-e4107db293e5 |
| {{tenantname}} | The name of your tenant. | if your Azure Ad B2C tenant url is muljin.onmicrosoft.com, replace `{{tenantname}}` with `muljin` |
| {{apibaseurl}} | The base URL of your API where the oidc metadata. This should be the same base url configured previously in the app settings | https://api.mydomain.com/ |

And then upload your custom policies to Azure Ad B2C in the Identity Framework section in the following order:
1. B2C_1A_TRUSTFRAMEWORKBASE.xml
1. B2C_1A_TRUSTFRAMEWORKLOCALIZATION.xml
1. B2C_1A_TRUSTFRAMEWORKEXTENSIONS.xml
1. B2C_1A_SIGNIN_WITH_EMAIL_MAGICLINK.xml


## Running
Once the libraries are added and your API is up and running, the following 2 endpoints will be available

| endpoint | description |
|----------|-------------|
| /oidc/.well-known/openid-configuration | Metadata endpoint |
| /oidc/.well-known/keys | JWKs endpoint |

### Creating a token
To create an exchangeable token, call the `IOidcService.BuildSerializedIdTokenAsync(string audience, int duration, string userEmail)` endpoint which will generate a JWT token with the given email as a claim.

This JWT can then be passed to the B2C_1A_SIGNIN_WITH_EMAIL_MAGICLINK custom policy which will exchange it for a JWT token generated by Azure AD B2C.

**Note:** this service returns a valid JWT token which can be used multiple times until expiration and should not be given directly to the caller, but sent via email or another secure communication channel. This JWT can either be sent in the email (less secure), or instead a randomly generated token can be emailed to the user, which will then be exchanged for the generated JWT token using a second API endpoint. By exchanging for a randomly generated token, you can ensure that it is only used once.

## Support
This package is provided as is under the license terms. Paid support packages are available by contacting us at info@muljin.com. For any additional help please try stackoverflow where ourselves or other members of the community might be able to help you.

Please do not open any Github issues unless you can demonstrate a clear, reproducible bug. If you are simply unable to get the project up and running, please check our upcoming blog posts found at https://www.muljin.com/blog or our youtube channel. Tutorials will be linked to this readme as they are released.

## Contributing
While we are honored and thank you for wanting to support our projects, due to limited resources we unfortunately we do not accept external pull requests or changes. Should you come across any bugs however, please add a Github issue and we will validate it and implement a fix where neccasary.
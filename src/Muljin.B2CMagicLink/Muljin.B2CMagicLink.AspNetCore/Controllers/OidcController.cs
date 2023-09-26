using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Muljin.B2CMagicLink.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OidcController : ControllerBase
    {
        private readonly IOidcService _odicService;

        public OidcController(IOidcService odicService)
        {
            _odicService = odicService;
        }

        [Route(".well-known/openid-configuration", Name = "OIDCMetadata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Metadata()
        {
            var meta = _odicService.GetSerializedOidcMetadata();
            return Content(meta, "application/json");
        }

        [Route(".well-known/keys", Name = "JWKS")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult JwksDocument()
        {
            var jwks = _odicService.GetSerializedJwks();
            return Content(jwks, "application/json");
        }
    }
}


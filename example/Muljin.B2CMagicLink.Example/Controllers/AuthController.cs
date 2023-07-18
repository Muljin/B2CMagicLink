using Microsoft.AspNetCore.Mvc;
using Muljin.B2CMagicLink.Example.Models;

namespace Muljin.B2CMagicLink.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpPost("signup")]    
    public IActionResult Signup([FromBody]CreateUserRequest req)
    {
        throw new NotImplementedException();
    }

    [HttpGet("signin")]
    public IActionResult SignIn([FromQuery]string email)
    {
        throw new NotImplementedException();
    }


}


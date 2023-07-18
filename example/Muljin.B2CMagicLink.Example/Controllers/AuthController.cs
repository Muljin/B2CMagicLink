using Microsoft.AspNetCore.Mvc;
using Muljin.B2CMagicLink.Example.Models;
using Muljin.B2CMagicLink.Example.Services;

namespace Muljin.B2CMagicLink.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly UsersService _usersService; 
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger,
        UsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
    }


    [HttpGet("signin")]
    public async Task<IActionResult> SignIn([FromQuery]string email)
    {
        await _usersService.SignupOrSigninAsync(email);
        return new OkResult();
    }

}


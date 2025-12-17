using Gateway.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class FaucetController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    
    public FaucetController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost]
    public IActionResult Dispense([FromBody] CreateTokenOptions options)
    {
        var token = _jwtTokenService.CreateToken(options);
        return Ok(token);
    }
}
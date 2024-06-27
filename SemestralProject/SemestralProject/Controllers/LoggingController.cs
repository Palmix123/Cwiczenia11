using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestralProject.Data.LoginDTOs;
using SemestralProject.Services.Login;

namespace SemestralProject.Controllers;

[ApiController]
[Route("user")]
public class LoggingController : ControllerBase
{
    private readonly IConfiguration Configuration;
    private readonly ILoggingService Service;

    public LoggingController(IConfiguration configuration, ILoggingService service)
    {
        Configuration = configuration;
        Service = service;
    }

    [Authorize(Roles = "admin")]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent(LoginRequest model)
    {
        await Service.RegisterUser(model);
        return Ok("User registered");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var token = await Service.LoginUser(loginRequest, Configuration);

        return Ok(new
        {
            accessToken = token.Item1,
            refreshToken = token.Item2
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(string refreshToken)
    {
        var result = await Service.RefreshAccessToken(refreshToken, Configuration);
        return Ok(new
        {
            accessToken = result.Item1,
            refreshToken = result.Item2
        });
    }
}
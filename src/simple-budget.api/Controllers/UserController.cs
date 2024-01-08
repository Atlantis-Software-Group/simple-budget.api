using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_budget.api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class UserController : ControllerBase
{
    public UserController(ILogger<UserController> logger)
    {
        Logger = logger;
    }

    public ILogger<UserController> Logger { get; }

    [HttpGet("claims",Name = "Claims")]
    public IEnumerable<string> GetUserClaims()
    {
        return User.Claims.Select(c => $"Claim Type: {c.Type} - Claim Value: {c.Value}")
                   .ToList();
    }

    [HttpGet("accesstoken",Name = "AccessToken")]
    public Task<string?> GetAccessToken()
    {
        return HttpContext.GetTokenAsync("access_token");
    }
}

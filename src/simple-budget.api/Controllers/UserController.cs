using Microsoft.AspNetCore.Mvc;

namespace simple_budget.api;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    public UserController(ILogger<UserController> logger)
    {
        Logger = logger;
    }

    public ILogger<UserController> Logger { get; }

    [HttpGet(Name = "UserClaims")]
    public IEnumerable<string> GetUserClaims()
    {
        return User.Claims.Select(c => $"Claim Type: {c.Type} - Claim Value: {c.Value}")
                   .ToList();
    }
}

using simple_budget.api.interfaces;

namespace simple_budget.api.Services;

public class UserService : IUserService
{
    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public IHttpContextAccessor HttpContextAccessor { get; }

    public long GetUserId()
    {
        if ( UserId.HasValue )
            return UserId.Value;

        HttpContext? context = HttpContextAccessor.HttpContext;

        if ( context is null )
        {
            throw new ArgumentNullException("HttpContext");
        }

        _ = long.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0", out long userId);

        UserId = userId;

        return UserId!.Value;
    }

    public long? UserId {
        get => _userId;
        set => _userId = value;
    }

    private long? _userId;
}

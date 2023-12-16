using System.Security.Claims;

namespace simple_budget.api;

public interface IGetUserInfoService
{
    Task<ClaimsPrincipal> GetUserInfo(ClaimsPrincipal user, string authorityUrl, string token);
}

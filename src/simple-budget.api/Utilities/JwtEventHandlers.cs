using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace simple_budget.api;

public static class JwtEventHandlers
{
    public static async Task OnJwtTokenValidated(TokenValidatedContext ctx)
    {
        IServiceScopeFactory scopeFactory = ctx.HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();
        using ( var scope = scopeFactory.CreateAsyncScope())
        {
            IGetUserInfoService getUserInfoService = scope.ServiceProvider.GetRequiredService<IGetUserInfoService>();
            string authorityUrl = ctx.Options.Authority!;   
            string token = ctx.Request.Headers["Authorization"]!;
            ctx.Principal = await getUserInfoService.GetUserInfo(ctx.Principal, authorityUrl, token);
        }
    }
}

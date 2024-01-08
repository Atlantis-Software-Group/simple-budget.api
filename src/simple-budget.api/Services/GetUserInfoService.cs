using System.Security.Claims;
using IdentityModel.Client;

namespace simple_budget.api;

public class GetUserInfoService : IGetUserInfoService
{
    public GetUserInfoService(ILogger<GetUserInfoService> logger, IHttpClientFactory httpClientFactory)
    {
        Logger = logger;
        HttpClientFactory = httpClientFactory;
    }

    public ILogger<GetUserInfoService> Logger { get; }
    public IHttpClientFactory HttpClientFactory { get; }
    public ClaimsPrincipal? User { get; set; } = null;

    public async Task<ClaimsPrincipal> GetUserInfo(ClaimsPrincipal user, string authorityUrl, string token)
    {
        User = user;
        Logger.LogInformation("Starting Get UserInfo - Token: {token}", token);
        
        string justToken = token.Split(' ')[1];

        if ( string.IsNullOrWhiteSpace(authorityUrl) )
            throw new ArgumentNullException(nameof(authorityUrl), "Authority URL cannot be null");
        
        if ( string.IsNullOrWhiteSpace(token) )
            throw new ArgumentNullException(nameof(token), "Token cannot be null");

        UserInfoResponse? userInfo = null;
        try
        {
            using ( HttpClient client = HttpClientFactory.CreateClient() )
            {
                DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(authorityUrl);

                userInfo = await client.GetUserInfoAsync(new UserInfoRequest{
                    Address = disco.UserInfoEndpoint,
                    Token = justToken
                });
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error encountered while fetching user claims");
            throw;
        }

        if ( userInfo is null )
            throw new NullReferenceException("User Information cannot be null");

        ClaimsIdentity newIdentity = new ClaimsIdentity(userInfo.Claims);
        Logger.LogInformation("User Information was fetched");
        User.AddIdentity(newIdentity);
        return User;
    }
}

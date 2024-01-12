using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using simple_budget.api.data;

namespace simple_budget.api;

public class ApiClaimsTranformerService : IClaimsTransformation
{
    public ApiClaimsTranformerService(ApplicationDbContext context, 
                                        IGetUserInfoService getUserInfoService, 
                                        IHttpContextAccessor httpContextAccessor,
                                        TimeProvider timeProvider,
                                        ILogger<ApiClaimsTranformerService> logger)
    {
        Context = context;
        GetUserInfoService = getUserInfoService;
        HttpContextAccessor = httpContextAccessor;
        TimeProvider = timeProvider;
        Logger = logger;
    }

    public ApplicationDbContext Context { get; }
    public IGetUserInfoService GetUserInfoService { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public TimeProvider TimeProvider { get; }
    public ILogger<ApiClaimsTranformerService> Logger { get; }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if ( principal.HasClaim(x => x.Type == ClaimTypes.Role) )
            return principal; 

        if ( !principal.HasClaim(x => x.Type == "id" ) )
            return principal;        

        string? identityId = principal.Claims.Where(c => c.Type == "id")
                                            .Select(c => c.Value)
                                            .SingleOrDefault();

        if ( string.IsNullOrWhiteSpace(identityId) )
            return principal;

        long userId = await Context.IdentityUserMappings.Where(iu => iu.IdentityUserId == identityId)
                                            .Select(iu => iu.UserId)
                                            .SingleOrDefaultAsync();
        ClaimsIdentity identity = new ClaimsIdentity();

        if ( userId == 0 )
        {
            long systemUserId = await Context.Users.Where(u => u.Name == "System User")
                                            .Select(u => u.Id)
                                            .SingleOrDefaultAsync();

            if ( systemUserId == 0 )
                return principal;

            string? email = principal.Claims.Where(c => c.Type == "email")
                                            .Select(c => c.Value)
                                            .SingleOrDefault();
            
            string? name = principal.Claims.Where(c => c.Type == "name")
                                            .Select(c => c.Value)
                                            .SingleOrDefault();

            if ( string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
                return principal;

            Role? userRole = await Context.Roles.Where(r => r.Name == "User")
                                                .FirstOrDefaultAsync();


            if ( userRole is null )
                return principal;

            identity.AddClaim(new Claim(ClaimTypes.Role, userRole.Name));

            UserRole newUserRole = new UserRole{ Role = userRole, RoleId = userRole.Id };

            DateTimeOffset currentUtc = TimeProvider.GetUtcNow();
            User user = new User() {
                Email = email,
                Name = name,
                UserRole = newUserRole,
                CreatedBy = systemUserId,
                CreatedOn = currentUtc.DateTime,
                ModifiedBy = systemUserId,                
                ModifiedOn = currentUtc.DateTime,
            };

            newUserRole.User = user;

            await Context.Users.AddAsync(user);

            // add mapping to the table
            IdentityUserMapping mapping = new IdentityUserMapping(identityId)
            {
                User = user
            };

            await Context.IdentityUserMappings.AddAsync(mapping);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error encountered while attempting to setup a new user: {@user}", user);
                throw;
            }
            userId = user.Id;
        }        
        else
        {            
            Role? userRole = await Context.Roles
                                            .Join(
                                                Context.UserRoles,
                                                r => r.Id,
                                                ur => ur.RoleId,
                                                (role, userRole) => new {
                                                    id = role.Id,
                                                    name = role.Name,
                                                    userId = userRole.UserId
                                                }
                                            )
                                            .Where(x => x.userId == userId)
                                            .Select(x => new Role(){
                                                Id = x.id,
                                                Name = x.name
                                            })
                                            .FirstOrDefaultAsync();

            if ( userRole is null )
                return principal;

            
            identity.AddClaim(new Claim(ClaimTypes.Role, userRole.Name));
        }
        
        identity.AddClaim(new Claim("UserId", userId.ToString()));
        principal.AddIdentity(identity);

        return principal;
    }
}

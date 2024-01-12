using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using asg.dbmigrator.SeedData.Services;
using asg.dbmigrator.SeedData.Attributes;
using simple_budget.api.data;

namespace asg.Data.Migrator.SeedData.Databases.ApplicationDb;

[MigrationName("20240107185243_disableRequiredOnUser")]
[SeedEnvironment("Development")]
[SeedEnvironment("Local")]
[DatabaseName("ApplicationDbContext")]
public class AddSystemUser : SeedDataService
{
	public AddSystemUser(ApplicationDbContext context, TimeProvider timeProvider, IConfiguration configuration, ILogger<AddSystemUser> logger) : base(configuration, logger)
	{
		Context = context;
		TimeProvider = timeProvider;
	}

	public ApplicationDbContext Context { get; }
	public TimeProvider TimeProvider { get; }

	public override async Task<bool> Seed()
	{
		//Seed data
		User user = new User
		{
			CreatedBy = -1,
			CreatedOn = TimeProvider.GetUtcNow().DateTime,
			ModifiedBy = -1,
			ModifiedOn = TimeProvider.GetUtcNow().DateTime,
			Email = "NotApplicable",
			Name = "System User"
		};

		await Context.Users.AddAsync(user);
		await Context.SaveChangesAsync();

		return true;
	}
}


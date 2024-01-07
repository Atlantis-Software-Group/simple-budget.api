using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using asg.dbmigrator.SeedData.Services;
using asg.dbmigrator.SeedData.Attributes;
using simple_budget.api.data;

namespace asg.Data.Migrator.SeedData.Databases.ApplicationDb;

[MigrationName("20240106031958_InitialCreate")]
[SeedEnvironment("Development")]
[SeedEnvironment("Local")]
[DatabaseName("ApplicationDbContext")]
public class AddRoles : SeedDataService
{
	public AddRoles(ApplicationDbContext context, IConfiguration configuration, ILogger<AddRoles> logger) : base(configuration, logger)
	{
		Context = context;
	}

	public ApplicationDbContext Context { get; }

	public override async Task<bool> Seed()
	{
		Role userRole = new Role() {
			Name = "User"
		};
		await Context.Roles.AddAsync(userRole);

		Role adminRole = new Role() {
			Name = "Admin"
		};
		await Context.Roles.AddAsync(adminRole);

		await Context.SaveChangesAsync();
      
      	return true;
	}
}


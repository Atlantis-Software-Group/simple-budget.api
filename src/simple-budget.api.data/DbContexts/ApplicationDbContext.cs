using Microsoft.EntityFrameworkCore;
using simple_budget.api.data.Transactions;

namespace simple_budget.api.data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles {get; set; }
    public DbSet<IdentityUserMapping> IdentityUserMappings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
}

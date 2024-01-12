using Microsoft.Extensions.Logging;
using simple_budget.api.interfaces;

namespace simple_budget.api.data.Transactions;

public class TransactionRepository : ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context, ILogger<TransactionRepository> logger, IUserService userService, TimeProvider timeProvider)
    {
        Context = context;
        Logger = logger;
        UserService = userService;
        TimeProvider = timeProvider;
    }

    public ApplicationDbContext Context { get; }
    public ILogger<TransactionRepository> Logger { get; }
    public IUserService UserService { get; }
    public TimeProvider TimeProvider { get; }

    public async Task<Transaction> SaveTransaction(Transaction transaction)
    {
        DateTimeOffset utc = TimeProvider.GetUtcNow();
        if ( transaction.Id == 0 )
        {
            transaction.CreatedBy = UserService.GetUserId();
            transaction.CreatedOn = utc.UtcDateTime;
        }

        transaction.ModifiedBy = UserService.GetUserId();
        transaction.ModifiedOn = utc.UtcDateTime;

        await Context.Transactions.AddAsync(transaction);
        try
        {
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error occured while creating a Transaction - {@Transaction}", transaction);
            throw;
        }
        
        return transaction;
    }
}

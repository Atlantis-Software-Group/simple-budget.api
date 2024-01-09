
using Microsoft.Extensions.Logging;

namespace simple_budget.api.data.Transactions;

public class TransactionRepository : ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context, ILogger<TransactionRepository> logger)
    {
        Context = context;
        Logger = logger;
    }

    public ApplicationDbContext Context { get; }
    public ILogger<TransactionRepository> Logger { get; }

    public async Task<Transaction> SaveTransaction(Transaction transaction)
    {
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

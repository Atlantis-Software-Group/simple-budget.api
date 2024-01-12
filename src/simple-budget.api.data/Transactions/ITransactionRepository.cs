namespace simple_budget.api.data.Transactions;

public interface ITransactionRepository
{
    Task<Transaction> SaveTransaction(Transaction transaction);
}

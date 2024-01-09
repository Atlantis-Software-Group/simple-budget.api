
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simple_budget.api.data.Transactions;

namespace simple_budget.api.Transactions;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class TransactionController : ControllerBase
{
    public TransactionController(ILogger<TransactionController> logger, ITransactionRepository transactionRepository)
    {
        Logger = logger;
        Repo = transactionRepository;
    }

    public ILogger<TransactionController> Logger { get; }
    public ITransactionRepository Repo { get; }

    [HttpPost("transaction")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        Transaction newTransaction = new Transaction {
            Name = request.Name!,
            Description = request.Description!,
            Notes = request.Notes!,
            Amount = request.Amount!.Value,
            Date = request.Date!.Value
        };

        await Repo.SaveTransaction(newTransaction);

        return Ok(newTransaction);
    }
}

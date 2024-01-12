
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simple_budget.api.data.Transactions;
using simple_budget.api.interfaces;
using simple_budget.api.Models;

namespace simple_budget.api.Transactions;

[ApiController]
[Route("api/transactions")]
[Authorize(Roles = "User")]
public class TransactionsController : ControllerBase
{
    public TransactionsController(ILogger<TransactionsController> logger, ITransactionRepository transactionRepository, IUserService userService, TimeProvider timeProvider)
    {
        Logger = logger;
        Repo = transactionRepository;
        UserService = userService;
        TimeProvider = timeProvider;
    }

    public ILogger<TransactionsController> Logger { get; }
    public ITransactionRepository Repo { get; }
    public IUserService UserService { get; }
    public TimeProvider TimeProvider { get; }

    [HttpPost("transaction")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        ApiResponse<TransactionReponse> response = new ApiResponse<TransactionReponse>();

        if ( !request.IsValid(TimeProvider, out IEnumerable<string> errors) )
        {
            response.Success = false;
            response.ErrorMessage = string.Join(", ", errors);
            return BadRequest(response);
        }

        Transaction newTransaction = new()
        {
            Name = request.Name!,
            Description = request.Description,
            Notes = request.Notes,
            Amount = request.Amount!.Value,
            Date = request.Date!.Value
        };

        try
        {
            await Repo.SaveTransaction(newTransaction);
            response.Success = true;
            response.Data = new TransactionReponse{
                Id = newTransaction.Id,
                Name = newTransaction.Name,
                Amount = newTransaction.Amount,
                Date = newTransaction.Date,
                Notes = newTransaction.Notes,
                Description = newTransaction.Description,
                CreatedBy = newTransaction.CreatedBy,
                CreatedOn = newTransaction.CreatedOn,
                ModifiedBy = newTransaction.ModifiedBy,
                ModifiedOn = newTransaction.ModifiedOn
            };
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error encountered while creating a Transaction: {@Transaction} - UserId: {userId}", 
                            newTransaction, 
                            UserService.GetUserId());
            
            response.Success = false;
            response.ErrorMessage = "Unable to save transaction. Please try again later.";
            return StatusCode(500, response);
        }

        return CreatedAtAction("GetTransaction", new { transactionId = newTransaction.Id }, response);
    }

    [HttpGet("transaction/{transactionId:long}", Name = "GetTransaction")]
    public Task<IActionResult> GetTransactionAsync(long transactionId)
    {
        throw new NotImplementedException();
    }
}

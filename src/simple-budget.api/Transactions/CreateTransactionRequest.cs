namespace simple_budget.api.Transactions;

public class CreateTransactionRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }
}

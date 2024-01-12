namespace simple_budget.api;

public class TransactionReponse
{  
    public long Id { get; set; }
    public DateTime Date { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }    

    public decimal Amount { get; set; }

    public string? Notes { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public long ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}

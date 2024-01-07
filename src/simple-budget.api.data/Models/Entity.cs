namespace simple_budget.api.data;

public class Entity
{
    public long Id { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public long ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}

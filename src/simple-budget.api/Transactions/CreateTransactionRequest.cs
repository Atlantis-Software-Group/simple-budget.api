using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace simple_budget.api.Transactions;

public class CreateTransactionRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }

    public bool IsValid(TimeProvider timeProvider,out IEnumerable<string> errors) 
    {
        List<string> _errors = new List<string>();
        

        bool isValid = true;

        if ( string.IsNullOrEmpty(Name) )
        {
            isValid = false;
            _errors.Add("Name cannot be null");
        }
        else if ( Name.Length > 512 )
        {
            isValid = false;
            _errors.Add("Name must be less than 512 characters.");
        }

        if ( Date  is null )
        {
            isValid = false;
            _errors.Add("Date cannot be null");
        }
        else if ( Date < timeProvider.GetLocalNow().AddMonths(-12) )
        {
            isValid = false;
            _errors.Add("Date cannot be more than a year in the past");
        }
        else if ( timeProvider.GetLocalNow().AddMonths(12) < Date)
        {
            isValid = false;
            _errors.Add("Date cannot be more than a year in the future");
        }

        if ( Amount is null )
        {
            isValid = false;
            _errors.Add("Amount cannot be null");
        }

        if ( Description is not null && Description.Length > 1024 )
        {
            isValid = false;
            _errors.Add("Description must be shorter than 1024 characters.");
        }

        if ( Notes is not null && Notes.Length > 4096 )
        {
            isValid = false;
            _errors.Add("Notes must be shorter than 4096 characters");
        }

        errors = _errors;
        return isValid;
    }
}

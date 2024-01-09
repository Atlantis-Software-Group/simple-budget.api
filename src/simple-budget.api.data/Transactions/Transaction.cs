using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_budget.api.data.Transactions;

[Table("Transactions")]
public class Transaction : Entity
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(512)]
    public string Name { get; set; } = null!;

    [MaxLength(1024)]
    public string? Description { get; set; }    

    [Required]
    public decimal Amount { get; set; }

    [MaxLength(4096)]
    public string? Notes { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_budget.api.data;

[Table("Roles")]
public class Role
{
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public UserRole? UserRole { get; set; } = null;
}

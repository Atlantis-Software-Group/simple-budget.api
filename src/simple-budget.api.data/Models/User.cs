using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_budget.api.data;

[Table("Users")]
public class User : Entity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(512)]
    public string Email { get; set; } = null!;

    public UserRole? UserRole { get; set; } = null;
}

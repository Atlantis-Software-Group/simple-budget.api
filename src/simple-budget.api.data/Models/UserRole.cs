using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_budget.api.data;

[Table("UserRoles")]
public class UserRole
{
    [Key]
    public long UserId { get; set; }
    public long RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

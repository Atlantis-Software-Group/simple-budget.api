using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_budget.api.data;

[Table("IdentityUserMapping")]
public class IdentityUserMapping(string identityUserId)
{

    [Key]
    [MaxLength(50)]
    public string IdentityUserId { get; set; } = identityUserId;

    [Required]
    public long UserId { get; set; }

    public User User { get; set; } = null!;
}

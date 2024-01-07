using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_budget.api.data;

[Table("IdentityUserMapping")]
public class IdentityUserMapping(string identityUserId, long userId)
{

    [Key]
    [MaxLength(30)]
    public string IdentityUserId { get; set; } = identityUserId;

    [Required]
    public long UserId { get; set; } = userId;

    public User User { get; set; } = null!;
}

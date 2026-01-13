using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordChecker.Data.Models;

[Index("UserId", Name = "IX_PasswordChecks_UserId")]
public partial class PasswordCheck
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("score")]
    public int Score { get; set; }

    [Column("level")]
    [StringLength(20)]
    public string Level { get; set; } = null!;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PasswordChecks")]
    public virtual User User { get; set; } = null!;
}

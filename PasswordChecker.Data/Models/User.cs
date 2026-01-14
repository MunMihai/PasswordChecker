using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Index("Email", Name = "UQ_User_Email", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "ACTIVE";

    [Column("role")]
    [StringLength(20)]
    public string Role { get; set; } = "CUSTOMER";

    [Column("balance", TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;

    [Column("varsta")]
    public int? Varsta { get; set; }

    [Column("gen")]
    [StringLength(10)]
    public string? Gen { get; set; }

    [Column("password_hash")]
    [StringLength(255)]
    public string? PasswordHash { get; set; }

    public virtual ICollection<PasswordCheck> PasswordChecks { get; set; } = new List<PasswordCheck>();
    public virtual Subscription? Subscription { get; set; }
}

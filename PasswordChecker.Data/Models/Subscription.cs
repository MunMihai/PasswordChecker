using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordChecker.Data.Models;

[Index("PlanId", Name = "IX_Subscription_PlanId")]
[Index("UserId", Name = "IX_Subscription_UserId")]
public partial class Subscription
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("plan_id")]
    public Guid PlanId { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("PlanId")]
    [InverseProperty("Subscriptions")]
    public virtual Plan Plan { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Subscription")]
    public virtual User User { get; set; } = null!;
}

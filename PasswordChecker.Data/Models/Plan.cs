using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordChecker.Data.Models;

public partial class Plan
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("price", TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }

    [Column("max_checks_per_day")]
    public int MaxChecksPerDay { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Models;

namespace PasswordChecker.Data.Context;

public partial class PasswordCheckerDbContext_CodeFirst : DbContext
{
    public PasswordCheckerDbContext_CodeFirst()
    {
    }

    public PasswordCheckerDbContext_CodeFirst(DbContextOptions<PasswordCheckerDbContext_CodeFirst> options)
        : base(options)
    {
    }

    public virtual DbSet<PasswordCheck> PasswordChecks { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordCheck>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordChecks).HasConstraintName("FK_PasswordChecks_User");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UX_Subscription_ActiveUser")
                .IsUnique()
                .HasFilter("([status]='ACTIVE')");

            entity.Property(e => e.Status).HasDefaultValue("ACTIVE");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscription_Plan");

            entity.HasOne(d => d.User).WithOne(p => p.Subscription).HasConstraintName("FK_Subscription_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("ACTIVE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

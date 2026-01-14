using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Context;
using PasswordChecker.Data.Models;

namespace PasswordChecker.Data.SeedData;

public static class DbSeeder
{
    public static async Task SeedAsync(PasswordCheckerDbContext_CodeFirst context)
    {
        // Verifică dacă baza de date are deja date
        if (await context.Plans.AnyAsync())
        {
            return; // Baza de date este deja populată
        }

        // Seed Plans
        var plans = new List<Plan>
        {
            new Plan
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Free",
                Price = 0,
                MaxChecksPerDay = 5,
                IsActive = true
            },
            new Plan
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Basic",
                Price = 9.99m,
                MaxChecksPerDay = 50,
                IsActive = true
            },
            new Plan
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Premium",
                Price = 19.99m,
                MaxChecksPerDay = 200,
                IsActive = true
            },
            new Plan
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Enterprise",
                Price = 49.99m,
                MaxChecksPerDay = 1000,
                IsActive = true
            },
            new Plan
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Legacy",
                Price = 5.99m,
                MaxChecksPerDay = 20,
                IsActive = false
            }
        };

        await context.Plans.AddRangeAsync(plans);

        // Seed Users
        var users = new List<User>
        {
            new User
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Email = "admin@passwordchecker.com",
                CreatedAt = DateTime.UtcNow.AddMonths(-6),
                Status = "ACTIVE",
                Role = "ADMIN",
                Balance = 1000.00m,
                Varsta = 35,
                Gen = "M",
                PasswordHash = "$2a$11$rpfaCdUldatN3vHsuqNLG.JaEZ0Ytjoxx.CbsmEmY/w2Q9pnCwp4m" // password: 123123
            },
            new User
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Email = "john.doe@example.com",
                CreatedAt = DateTime.UtcNow.AddMonths(-3),
                Status = "ACTIVE",
                Role = "CUSTOMER",
                Balance = 25.50m,
                Varsta = 28,
                Gen = "M"
            },
            new User
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Email = "jane.smith@example.com",
                CreatedAt = DateTime.UtcNow.AddMonths(-2),
                Status = "ACTIVE",
                Role = "CUSTOMER",
                Balance = 0.00m,
                Varsta = 32,
                Gen = "F"
            },
            new User
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Email = "mike.wilson@example.com",
                CreatedAt = DateTime.UtcNow.AddMonths(-1),
                Status = "ACTIVE",
                Role = "CUSTOMER",
                Balance = 50.00m,
                Varsta = 45,
                Gen = "M"
            },
            new User
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Email = "sarah.johnson@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                Status = "ACTIVE",
                Role = "CUSTOMER",
                Balance = 10.00m,
                Varsta = 26,
                Gen = "F"
            },
            new User
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Email = "inactive.user@example.com",
                CreatedAt = DateTime.UtcNow.AddMonths(-4),
                Status = "INACTIVE",
                Role = "CUSTOMER",
                Balance = 0.00m,
                Varsta = 30,
                Gen = "M"
            }
        };

        await context.Users.AddRangeAsync(users);

        // Seed Subscriptions
        var subscriptions = new List<Subscription>
        {
            new Subscription
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // john.doe
                PlanId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Basic
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                Status = "ACTIVE"
            },
            new Subscription
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), // jane.smith
                PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Free
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)),
                EndDate = null,
                Status = "ACTIVE"
            },
            new Subscription
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), // mike.wilson
                PlanId = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Premium
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                Status = "ACTIVE"
            },
            new Subscription
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"),
                UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), // sarah.johnson
                PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Free
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
                EndDate = null,
                Status = "ACTIVE"
            },
            new Subscription
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005"),
                UserId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), // inactive.user
                PlanId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Basic
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-4)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                Status = "EXPIRED"
            }
        };

        await context.Subscriptions.AddRangeAsync(subscriptions);

        // Seed PasswordChecks
        var passwordChecks = new List<PasswordCheck>
        {
            // Password checks pentru john.doe (Basic plan - 50 checks/day)
            new PasswordCheck
            {
                Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Score = 85,
                Level = "STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Score = 45,
                Level = "WEAK",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Score = 72,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("11111111-0000-0000-0000-000000000004"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Score = 95,
                Level = "VERY_STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("11111111-0000-0000-0000-000000000005"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Score = 30,
                Level = "VERY_WEAK",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            // Password checks pentru jane.smith (Free plan - 5 checks/day)
            new PasswordCheck
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Score = 60,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Score = 78,
                Level = "STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000003"),
                UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Score = 88,
                Level = "STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            // Password checks pentru mike.wilson (Premium plan - 200 checks/day)
            new PasswordCheck
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Score = 92,
                Level = "VERY_STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Score = 55,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000003"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Score = 100,
                Level = "VERY_STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000004"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Score = 40,
                Level = "WEAK",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000005"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Score = 75,
                Level = "STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000006"),
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Score = 65,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            // Password checks pentru sarah.johnson (Free plan - 5 checks/day)
            new PasswordCheck
            {
                Id = Guid.Parse("44444444-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Score = 50,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("44444444-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Score = 68,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("44444444-0000-0000-0000-000000000003"),
                UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Score = 82,
                Level = "STRONG",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            // Password checks pentru inactive.user
            new PasswordCheck
            {
                Id = Guid.Parse("55555555-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Score = 35,
                Level = "WEAK",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new PasswordCheck
            {
                Id = Guid.Parse("55555555-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Score = 58,
                Level = "MEDIUM",
                CreatedAt = DateTime.UtcNow.AddMonths(-3).AddDays(2)
            }
        };

        await context.PasswordChecks.AddRangeAsync(passwordChecks);

        await context.SaveChangesAsync();
    }
}

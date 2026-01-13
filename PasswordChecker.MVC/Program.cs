using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Context;
using PasswordChecker.Data.Repositories;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Data.Services;
using PasswordChecker.Server.Services;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services
                .AddAuthentication("AdminCookie")
                .AddCookie("AdminCookie", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                });

            builder.Services.AddAuthorization();


            builder.Services.AddDbContext<PasswordCheckerDbContext_CodeFirst>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();


            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();   
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

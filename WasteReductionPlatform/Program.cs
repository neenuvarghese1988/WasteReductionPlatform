using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Middleware;
using WasteReductionPlatform.Models;
using WasteReductionPlatform.Services;

namespace WasteReductionPlatform
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WasteReductionContext")));
            builder.Services.AddScoped<NotificationService>();
			builder.Services.AddHostedService<NotificationBackgroundService>();
			builder.Services.AddDefaultIdentity<User>(options =>
            {
               // options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

			//builder.Services.ConfigureApplicationCookie(options =>
			//{
			//    options.Cookie.HttpOnly = true;
			//    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Persistent login duration
			//    options.SlidingExpiration = false;
			//    options.Cookie.IsEssential = true;
			//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			//});
			builder.Services.AddDistributedMemoryCache();

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Register the email sender service
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            // Use custom error handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseCookiePolicy(new CookiePolicyOptions
            //{
            //    MinimumSameSitePolicy = SameSiteMode.Lax, // Ensure cookies are properly handled cross-site
            //    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
            //});
            app.UseAuthentication();
            app.UseAuthorization();
			app.UseSession();
			app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Bind Admin User Settings from appsettings.json
            var adminUserSettings = builder.Configuration.GetSection("AdminUser").Get<AdminUserSettings>();

            await InitializeRolesAndAdminAsync(app, adminUserSettings);

            app.Run();
        }

        //private static async Task InitializeRolesAndAdminAsync(WebApplication app)
        //{
        //    using var scope = app.Services.CreateScope();
        //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        //    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        //    string[] roles = { "Admin", "User" };

        //    foreach (var role in roles)
        //    {
        //        if (!await roleManager.RoleExistsAsync(role))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(role));
        //            logger.LogInformation($"Role '{role}' created successfully.");
        //        }
        //    }

        //    // Optionally remove admin creation from here
        //}


        private static async Task InitializeRolesAndAdminAsync(WebApplication app, AdminUserSettings adminUserSettings)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            string[] roles = { "Admin", "User" };

            try
            {
                // Create roles if they do not exist
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                }

                // Create admin user if it does not exist
                var adminUser = await userManager.FindByNameAsync(adminUserSettings.UserName);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminUserSettings.UserName,
                        IsAdmin = adminUserSettings.IsAdmin,
                        UserType = Enum.Parse<UserType>(adminUserSettings.UserType),
                        StreetAddress = adminUserSettings.StreetAddress,
                        City = adminUserSettings.City,
                        Province = adminUserSettings.Province,
                        PostalCode = adminUserSettings.PostalCode,
                        EmailConfirmed = true // Confirm email here
                    };

                    var result = await userManager.CreateAsync(adminUser, adminUserSettings.Password);
                    if (result.Succeeded)

                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Admin user created and assigned to 'Admin' role.");
                    }
                    else
                    {
                        logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else if (!adminUser.EmailConfirmed)
                {
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);
                }
                else
                {
                    logger.LogInformation("Admin user already exists.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the role and admin user initialization.");
            }
        }
    }
}

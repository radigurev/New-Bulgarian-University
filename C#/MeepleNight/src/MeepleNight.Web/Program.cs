using FluentValidation;
using MeepleNight.Data;
using MeepleNight.Data.Repositories;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services;
using MeepleNight.Services.Mapping;
using MeepleNight.Services.Statistics;
using MeepleNight.Services.Validators;
using MeepleNight.Web.Infrastructure.Seeding;
using MeepleNight.Web.Infrastructure.Seeding.Interfaces;
using MeepleNight.Web.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services));

// 2. Database
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<MeepleDbContext>(options =>
    options.UseSqlServer(connectionString,
        sql => sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Identity (cookie-based, with roles)
builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireLowercase = false;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<MeepleDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "MeepleNight.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

// 4. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 5. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateGameRequestValidator>();

// 6. Repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameNightRepository, GameNightRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 7. Application services
builder.Services.AddScoped<IGameAdminService, GameAdminService>();
builder.Services.AddScoped<IGameQueryService, GameQueryService>();
builder.Services.AddScoped<IGameNightService, GameNightService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IPlayNextSuggestionService, PlayNextSuggestionService>();

// 8. CurrentUser accessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

// 8b. Dev data seeder (only used in Development; registered unconditionally for testability)
builder.Services.AddScoped<IDevDataSeeder, DevDataSeeder>();

// 9. MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserLogContextMiddleware>();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await SeedDatabaseAsync(app);
if (app.Environment.IsDevelopment())
{
    await SeedDevelopmentDataAsync(app);
}

app.Run();


// -------------- helpers --------------

static async Task SeedDatabaseAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;

    MeepleDbContext db = services.GetRequiredService<MeepleDbContext>();
    await db.Database.MigrateAsync();

    RoleManager<IdentityRole<Guid>> roleManager =
        services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    UserManager<User> userManager =
        services.GetRequiredService<UserManager<User>>();
    IConfiguration configuration =
        services.GetRequiredService<IConfiguration>();

    // Roles
    foreach (string roleName in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    // Bootstrap admin
    string adminEmail = configuration["Bootstrap:AdminEmail"] ?? "admin@meeplenight.local";
    string adminPassword = configuration["Bootstrap:AdminPassword"] ?? "Admin123!";
    string adminDisplayName = configuration["Bootstrap:AdminDisplayName"] ?? "Administrator";

    User? existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        User admin = new()
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            DisplayName = adminDisplayName,
            CreatedAtUtc = DateTime.UtcNow
        };
        IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    // Seed games (only if catalog is empty)
    if (!await db.Games.AnyAsync())
    {
        db.Games.AddRange(
            new Game { Title = "Catan", Description = "Classic resource-trading strategy.", MinPlayers = 3, MaxPlayers = 4, AverageDurationMinutes = 90, Category = MeepleNight.Domain.Enums.GameCategory.Strategy, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Game { Title = "Wingspan", Description = "Engine-builder about birds.", MinPlayers = 1, MaxPlayers = 5, AverageDurationMinutes = 70, Category = MeepleNight.Domain.Enums.GameCategory.Strategy, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Game { Title = "Codenames", Description = "Word-association party game for teams.", MinPlayers = 2, MaxPlayers = 8, AverageDurationMinutes = 30, Category = MeepleNight.Domain.Enums.GameCategory.Party, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Game { Title = "Pandemic", Description = "Cooperative game; cure diseases together.", MinPlayers = 2, MaxPlayers = 4, AverageDurationMinutes = 60, Category = MeepleNight.Domain.Enums.GameCategory.Cooperative, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Game { Title = "Dixit", Description = "Storytelling and abstract imagery.", MinPlayers = 3, MaxPlayers = 6, AverageDurationMinutes = 45, Category = MeepleNight.Domain.Enums.GameCategory.Family, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Game { Title = "Azul", Description = "Tile-laying abstract.", MinPlayers = 2, MaxPlayers = 4, AverageDurationMinutes = 45, Category = MeepleNight.Domain.Enums.GameCategory.Abstract, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }
}

static async Task SeedDevelopmentDataAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    IDevDataSeeder seeder = scope.ServiceProvider.GetRequiredService<IDevDataSeeder>();
    await seeder.SeedAsync();
}

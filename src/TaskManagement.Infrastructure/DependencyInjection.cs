using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;
using TaskManagement.Infrastructure.Identity;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Interceptors;
using TaskManagement.Infrastructure.Persistence.Seeding;
using TaskManagement.Infrastructure.Persistence.Repositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.Infrastructure.Settings;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<SeedDataSettings>(configuration.GetSection(SeedDataSettings.SectionName));

        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(3));
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
        {
            opts.Password.RequiredLength = 8;
            opts.Password.RequireDigit = true;
            opts.Password.RequireLowercase = true;
            opts.Password.RequireUppercase = true;
            opts.Password.RequireNonAlphanumeric = false;
            opts.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddSingleton<ICorrelationIdService, CorrelationIdService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<MasterDataSeederOrchestrator>();

        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}

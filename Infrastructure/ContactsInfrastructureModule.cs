using System.Text;
using AppCore.Authorization;
using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Seeders; 

namespace Infrastructure;

public static class ContactsInfrastructureModule
{
    public static IServiceCollection AddContactsEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CrmDb")));

        // Register Identity
        services.AddIdentity<CrmUser, CrmRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<ContactsDbContext>()
        .AddDefaultTokenProviders();

        // Register Repositories
        services.AddScoped<IPersonRepository, EfPersonRepository>();
        services.AddScoped<ICompanyRepository, EfCompanyRepository>();
        services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();

        // Register Unit of Work
        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();

        // Register Services
        services.AddScoped<IPersonService, PersonService>();

        // Register Auth Service
        services.AddScoped<IAuthService, AuthService>();
        // Register Seeders
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();

        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            // Policy: AdminOnly
            options.AddPolicy(CrmPolicies.AdminOnly.Name(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));

            // Policy: SalesAccess (Administrator, SalesManager, Salesperson)
            options.AddPolicy(CrmPolicies.SalesAccess.Name(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString(),
                    UserRole.Salesperson.ToString()));

            // Policy: SalesManagerAccess (Administrator, SalesManager)
            options.AddPolicy(CrmPolicies.SalesManagerAccess.Name(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString()));

            // Policy: SupportAccess (Administrator, SupportAgent)
            options.AddPolicy(CrmPolicies.SupportAccess.Name(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SupportAgent.ToString()));

            // Policy: ReadOnlyAccess (any authenticated user with any role)
            options.AddPolicy(CrmPolicies.ReadOnlyAccess.Name(), policy =>
                policy.RequireAuthenticatedUser());

            // Policy: ActiveUser (authenticated + status claim)
            options.AddPolicy(CrmPolicies.ActiveUser.Name(), policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("status", SystemUserStatus.Active.ToString()));

            // Policy: SalesDepartment (department claim)
            options.AddPolicy(CrmPolicies.SalesDepartment.Name(), policy =>
                policy.RequireClaim("department", "Sales"));

            // Default policy - any authenticated user
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Fallback policy - any authenticated user
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    public static IServiceCollection AddContactsMemoryModule(
        this IServiceCollection services)
    {
        services.AddSingleton<IPersonRepository, Infrastructure.Memory.MemoryPersonRepository>();
        services.AddSingleton<ICompanyRepository, Infrastructure.Memory.MemoryCompanyRepository>();
        services.AddSingleton<IOrganizationRepository, Infrastructure.Memory.MemoryOrganizationRepository>();
        services.AddSingleton<IContactUnitOfWork, Infrastructure.Memory.MemoryContactUnitOfWork>();
        services.AddSingleton<IPersonService, Infrastructure.Memory.MemoryPersonService>();

        return services;
    }
}
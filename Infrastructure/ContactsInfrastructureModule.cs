using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }

    public static IServiceCollection AddContactsMemoryModule(
        this IServiceCollection services)
    {
        // Register Memory Repositories
        services.AddSingleton<IPersonRepository, Infrastructure.Memory.MemoryPersonRepository>();
        services.AddSingleton<ICompanyRepository, Infrastructure.Memory.MemoryCompanyRepository>();
        services.AddSingleton<IOrganizationRepository, Infrastructure.Memory.MemoryOrganizationRepository>();
        services.AddSingleton<IContactUnitOfWork, Infrastructure.Memory.MemoryContactUnitOfWork>();
        services.AddSingleton<IPersonService, Infrastructure.Memory.MemoryPersonService>();

        return services;
    }
}
using AppCore.Interfaces;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seeders;

public class IdentityDbSeeder : IDataSeeder
{
    public int Order => 1;

    private readonly UserManager<CrmUser> _userManager;
    private readonly RoleManager<CrmRole> _roleManager;
    private readonly ILogger<IdentityDbSeeder> _logger;

    public IdentityDbSeeder(
        UserManager<CrmUser> userManager,
        RoleManager<CrmRole> roleManager,
        ILogger<IdentityDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new CrmRole(UserRole.Administrator.ToString(), "Full system access."),
            new CrmRole(UserRole.SalesManager.ToString(), "Manage sales team."),
            new CrmRole(UserRole.Salesperson.ToString(), "Handle customers and sales opportunities."),
            new CrmRole(UserRole.SupportAgent.ToString(), "Handle support tickets."),
            new CrmRole(UserRole.ReadOnly.ToString(), "Read-only access.")
        };

        foreach (var role in roles)
        {
            if (await _roleManager.RoleExistsAsync(role.Name!))
                continue;

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                _logger.LogError("Error creating role {Role}: {Errors}", role.Name, result.Errors);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new SeedUser(
                Id: "F5BADE14-6CC8-42A2-9A44-9842DA2D9280",
                Email: "admin@crm.pl",
                FirstName: "Adam",
                LastName: "Administrator",
                Department: "IT",
                Password: "Admin@123!",
                Role: UserRole.Administrator
            ),
            new SeedUser(
                Id: "93A7FFDD-057F-4021-9C68-FE06951FFA65",
                Email: "jan.kowalski@crm.pl",
                FirstName: "Jan",
                LastName: "Kowalski",
                Department: "Sales",
                Password: "Manager@123!",
                Role: UserRole.SalesManager
            ),
            new SeedUser(
                Id: "3D4769E2-1C75-43E1-A5BB-1F71C68E9F57",
                Email: "anna.nowak@crm.pl",
                FirstName: "Anna",
                LastName: "Nowak",
                Department: "Sales",
                Password: "Sales@123!",
                Role: UserRole.Salesperson
            ),
            new SeedUser(
                Id: "0E136AB2-1A6A-4A16-938D-84DFB0F64BBA",
                Email: "piotr.wisniewski@crm.pl",
                FirstName: "Piotr",
                LastName: "Wiśniewski",
                Department: "Sales",
                Password: "Piotr123!",
                Role: UserRole.Salesperson
            ),
            new SeedUser(
                Id: "76B253D6-C16C-470A-943C-92F314A090F2",
                Email: "maria.wojcik@crm.pl",
                FirstName: "Maria",
                LastName: "Wójcik",
                Department: "Support",
                Password: "Support@123!",
                Role: UserRole.SupportAgent
            ),
            new SeedUser(
                Id: "E90A39C9-9CE2-400A-8A7B-8CF300D3B292",
                Email: "tomasz.kaminski@crm.pl",
                FirstName: "Tomasz",
                LastName: "Kamiński",
                Department: "Management",
                Password: "Readonly@123!",
                Role: UserRole.ReadOnly
            )
        };

        foreach (var seedUser in users)
        {
            await CreateUserAsync(seedUser);
        }
    }

    private async Task CreateUserAsync(SeedUser seedUser)
    {
        if (await _userManager.FindByEmailAsync(seedUser.Email) is not null)
        {
            _logger.LogInformation("User {Email} already exists - skipping.", seedUser.Email);
            return;
        }

        var user = new CrmUser
        {
            Id = seedUser.Id,
            UserName = seedUser.Email,
            Email = seedUser.Email,
            FirstName = seedUser.FirstName,
            LastName = seedUser.LastName,
            FullName = $"{seedUser.FirstName} {seedUser.LastName}",
            Department = seedUser.Department,
            Status = SystemUserStatus.Active,
            EmailConfirmed = true,
            LockoutEnabled = false,
            CreatedAt = DateTime.UtcNow
        };

        user.Activate();

        var createResult = await _userManager.CreateAsync(user, seedUser.Password);
        if (!createResult.Succeeded)
        {
            _logger.LogError("Error creating user {Email}: {Errors}", user.Email, createResult.Errors);
            return;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, seedUser.Role.ToString());
        if (!roleResult.Succeeded)
        {
            _logger.LogError("Error assigning role {Role} to {Email}: {Errors}",
                seedUser.Role, seedUser.Email, roleResult.Errors);
        }

        _logger.LogInformation("Created user {Email} with role {Role}.", seedUser.Email, seedUser.Role);
    }
}

internal record SeedUser(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Department,
    string Password,
    UserRole Role
);
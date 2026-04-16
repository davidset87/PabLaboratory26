using AppCore.Enums;
using AppCore.Models;
using AppCore.ValueObjects;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }  // ← AJOUTEZ CETTE LIGNE

    public ContactsDbContext() { }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=crm.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuration for CrmUser
        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Configuration for CrmRole
        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(20);
        });

        // Configuration for RefreshToken
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Token).IsUnique();
            entity.Property(r => r.UserId).IsRequired();
        });

        // TPH inheritance mapping
        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        // Contact configuration - Address as owned type
        builder.Entity<Contact>(entity =>
        {
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.Phone).HasMaxLength(20);
            
            entity.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Id);
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Type).HasConversion<string>();
                
                address.OwnsOne(a => a.Country, country =>
                {
                    country.Property(c => c.Name).HasColumnName("CountryName").HasMaxLength(100);
                    country.Property(c => c.Code).HasColumnName("CountryCode").HasMaxLength(10);
                });
            });
        });

        // Person configuration
        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();
            entity.Property(p => p.Status).HasConversion<string>();
        });

        // Relationship: Person -> Employer (Company)
        builder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(e => e.Employees)
            .HasForeignKey(p => p.EmployerId);

        // Relationship: Organization -> Members
        builder.Entity<Organization>()
            .HasMany(o => o.Members)
            .WithOne(p => p.Organization)
            .HasForeignKey(p => p.OrganizationId);

        // Seed data with STATIC values
        var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var addressId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var person1Id = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f");
        var person2Id = Guid.Parse("B4DCB17C-F875-43F8-9D66-36597895A466");
        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");

        // Seed Company
        builder.Entity<Company>().HasData(
            new Company
            {
                Id = companyId,
                Name = "WSEI",
                OrganizationType = AppCore.Enums.OrganizationType.Employee,
                Email = "biuro@wsei.edu.pl",
                Phone = "123567123",
                Status = ContactStatus.Active,
                CreatedAt = fixedDate,
                UpdatedAt = fixedDate
            }
        );

        // Seed Persons
        builder.Entity<Person>().HasData(
            new
            {
                Id = person1Id,
                FirstName = "Adam",
                LastName = "Nowak",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "adam@wsei.edu.pl",
                Phone = "123456789",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Programista",
                CreatedAt = fixedDate,
                UpdatedAt = fixedDate
            },
            new
            {
                Id = person2Id,
                FirstName = "Ewa",
                LastName = "Kowalska",
                Gender = Gender.Female,
                Status = ContactStatus.Blocked,
                Email = "ewa@wsei.edu.pl",
                Phone = "123123123",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Tester",
                CreatedAt = fixedDate,
                UpdatedAt = fixedDate
            }
        );

        // Seed Address data
        builder.Entity<Contact>().OwnsOne(c => c.Address).HasData(new
        {
            ContactId = person1Id,
            Id = addressId,
            Street = "ul. Św. Filipa 17",
            City = "Kraków",
            PostalCode = "25-009",
            Type = AddressType.Home,
            CountryName = "Poland",
            CountryCode = "PL"
        });
    }
}
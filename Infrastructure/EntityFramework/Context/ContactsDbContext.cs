using AppCore.Enums;
using AppCore.Models;
using AppCore.ValueObjects;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// Alias pour résoudre l'ambiguïté
using RemovedContactEntity = Infrastructure.EntityFramework.Entities.RemovedContact;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RemovedContactEntity> RemovedContacts { get; set; }  // Utilise l'alias

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

        // Configuration for RemovedContact
        builder.Entity<RemovedContactEntity>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.OriginalId).IsRequired().HasMaxLength(36);
            entity.Property(r => r.RemovedByUserId).IsRequired().HasMaxLength(36);
            entity.Property(r => r.RemovedByUserEmail).HasMaxLength(200);
            entity.Property(r => r.DeduplicationReason).HasMaxLength(500);
            entity.Property(r => r.DeduplicationStrategy).HasMaxLength(50);
            entity.Property(r => r.FirstName).HasMaxLength(100);
            entity.Property(r => r.LastName).HasMaxLength(100);
            entity.Property(r => r.Email).HasMaxLength(200);
            entity.Property(r => r.Phone).HasMaxLength(20);
            entity.HasIndex(r => r.RemovedByUserId);
            entity.HasIndex(r => r.RemovedAt);
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
    }
}
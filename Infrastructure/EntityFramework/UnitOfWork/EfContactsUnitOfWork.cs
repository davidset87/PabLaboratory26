using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.UnitOfWork;

public class EfContactsUnitOfWork : IContactUnitOfWork
{
    private readonly IPersonRepository _personRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ContactsDbContext _context;

    public EfContactsUnitOfWork(
        IPersonRepository personRepository,
        ICompanyRepository companyRepository,
        IOrganizationRepository organizationRepository,
        ContactsDbContext context)
    {
        _personRepository = personRepository;
        _companyRepository = companyRepository;
        _organizationRepository = organizationRepository;
        _context = context;
    }

    public IPersonRepository Persons => _personRepository;
    public ICompanyRepository Companies => _companyRepository;
    public IOrganizationRepository Organizations => _organizationRepository;

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryContactUnitOfWork : IContactUnitOfWork
{
    private readonly IPersonRepository _persons;
    private readonly ICompanyRepository _companies;
    private readonly IOrganizationRepository _organizations;

    public MemoryContactUnitOfWork(
        IPersonRepository persons,
        ICompanyRepository companies,
        IOrganizationRepository organizations)
    {
        _persons = persons;
        _companies = companies;
        _organizations = organizations;
    }

    public IPersonRepository Persons => _persons;
    public ICompanyRepository Companies => _companies;
    public IOrganizationRepository Organizations => _organizations;

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}
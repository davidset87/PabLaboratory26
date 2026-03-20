namespace AppCore.Repositories;

public interface IContactUnitOfWork : IAsyncDisposable
{
    IPersonRepository Persons { get; }
    ICompanyRepository Companies { get; }
    IOrganizationRepository Organizations { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
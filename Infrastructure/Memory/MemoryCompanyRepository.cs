using AppCore.Models;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryCompanyRepository : MemoryGenericRepository<Company>, ICompanyRepository
{
    public Task<IEnumerable<Company>> FindByNameAsync(string namePattern)
    {
        throw new NotImplementedException();
    }

    public Task<Company?> FindByTaxIdAsync(string taxId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }
}
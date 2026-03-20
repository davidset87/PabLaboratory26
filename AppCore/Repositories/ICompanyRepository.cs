using AppCore.Models;

namespace AppCore.Repositories;

public interface ICompanyRepository : IGenericRepositoryAsync<Company>
{
    Task<IEnumerable<Company>> FindByNameAsync(string namePattern);
    Task<Company?> FindByTaxIdAsync(string taxId);
    Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId);
}
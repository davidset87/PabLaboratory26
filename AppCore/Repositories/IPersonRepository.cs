using AppCore.Models;

namespace AppCore.Repositories;

public interface IPersonRepository : IGenericRepositoryAsync<Person>
{
    Task<IEnumerable<Person>> GetEmployeesOfCompanyAsync(Guid companyId);
    Task<IEnumerable<Person>> GetPeopleFromSameOrganizationAsync(Guid organizationId);
}
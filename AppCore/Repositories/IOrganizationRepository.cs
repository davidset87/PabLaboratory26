using AppCore.Enums;
using AppCore.Models;

namespace AppCore.Repositories;

public interface IOrganizationRepository : IGenericRepositoryAsync<Organization>
{
    Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type);
    Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId);
}
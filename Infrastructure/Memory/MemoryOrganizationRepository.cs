using AppCore.Enums;
using AppCore.Models;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryOrganizationRepository : MemoryGenericRepository<Organization>, IOrganizationRepository
{
    public Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        throw new NotImplementedException();
    }
}
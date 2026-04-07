using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository : EfGenericRepository<Person>, IPersonRepository
{
    private readonly ContactsDbContext _context;

    public EfPersonRepository(ContactsDbContext context)
        : base(context.People)
    {
        _context = context;
    }

    public async Task<IEnumerable<Person>> GetEmployeesOfCompanyAsync(Guid companyId)
    {
        return await _dbSet
            .Where(p => p.EmployerId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetPeopleFromSameOrganizationAsync(Guid organizationId)
    {
        return await _dbSet
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();
    }
}
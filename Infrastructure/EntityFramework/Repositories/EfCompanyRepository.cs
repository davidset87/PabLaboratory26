using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfCompanyRepository : EfGenericRepository<Company>, ICompanyRepository
{
    private readonly ContactsDbContext _context;

    public EfCompanyRepository(ContactsDbContext context)
        : base(context.Companies)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> FindByNameAsync(string namePattern)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(namePattern))
            .ToListAsync();
    }

    public async Task<Company?> FindByTaxIdAsync(string taxId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.TaxId == taxId);
    }

    public async Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        return await _context.People
            .Where(p => p.EmployerId == companyId)
            .ToListAsync();
    }
}
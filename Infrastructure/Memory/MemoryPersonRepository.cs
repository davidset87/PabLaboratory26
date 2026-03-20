using AppCore.Models;
using AppCore.Repositories;
using AppCore.Enums;
using AppCore.ValueObjects;

namespace Infrastructure.Memory;

public class MemoryPersonRepository : MemoryGenericRepository<Person>, IPersonRepository
{
    public MemoryPersonRepository()
    {
        var person1 = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam.nowak@example.com",
            Phone = "123-456-789",
            Gender = Gender.Male,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Address = new Address
            {
                Id = Guid.NewGuid(),
                Street = "Ul. Przykładowa 1",
                City = "Warszawa",
                PostalCode = "00-001",
                Country = new Country { Name = "Polska", Code = "PL" },
                Type = AddressType.Home
            }
        };

        var person2 = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "Anna",
            LastName = "Kowalska",
            Email = "anna.kowalska@example.com",
            Phone = "987-654-321",
            Gender = Gender.Female,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Address = new Address
            {
                Id = Guid.NewGuid(),
                Street = "Ul. Testowa 2",
                City = "Kraków",
                PostalCode = "30-001",
                Country = new Country { Name = "Polska", Code = "PL" },
                Type = AddressType.Home
            }
        };

        _data[person1.Id] = person1;
        _data[person2.Id] = person2;
    }
    
    public async Task<IEnumerable<Person>> GetEmployeesOfCompanyAsync(Guid companyId)
    {
        var result = _data.Values.Where(p => p.EmployerId == companyId);
        return await Task.FromResult(result);
    }
    
    public async Task<IEnumerable<Person>> GetPeopleFromSameOrganizationAsync(Guid organizationId)
    {
        var result = _data.Values.Where(p => p.OrganizationId == organizationId);
        return await Task.FromResult(result);
    }
}
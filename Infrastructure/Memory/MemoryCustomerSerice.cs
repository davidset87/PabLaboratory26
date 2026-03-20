using AppCore.Interfaces;
using AppCore.Models;
using AppCore.Enums;
using AppCore.ValueObjects;

namespace Infrastructure.Memory;

public class MemoryCustomerService : ICustomerService
{
    public IEnumerable<Customer> GetCustomers()
    {
        return new List<Customer>
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "a@wsei.edu.pl",
                Phone = "111",
                Address = new Address
                {
                    Id = Guid.NewGuid(),
                    Street = "Ul. Przykładowa 1",
                    City = "Warszawa",
                    PostalCode = "00-001",
                    Country = new Country { Name = "Polska", Code = "PL" }
                },
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "b@wsei.edu.pl",
                Phone = "444-555-666",
                Address = new Address
                {
                    Id = Guid.NewGuid(),
                    Street = "Ul. Testowa 2",
                    City = "Kraków",
                    PostalCode = "30-001",
                    Country = new Country { Name = "Polska", Code = "PL" }
                },
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }

    public Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return Task.FromResult(GetCustomers());
    }
}
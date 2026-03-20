using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICustomerService
{
    IEnumerable<Customer> GetCustomers();
    Task<IEnumerable<Customer>> GetCustomersAsync();
}
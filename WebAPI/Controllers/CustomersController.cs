using AppCore.Interfaces;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetCustomers()
    {
        var customers = _customerService.GetCustomers();
        return Ok(customers);
    }

    [HttpGet("async")]
    public async Task<IActionResult> GetCustomersAsync()
    {
        var customers = await _customerService.GetCustomersAsync();
        return Ok(customers);
    }
}
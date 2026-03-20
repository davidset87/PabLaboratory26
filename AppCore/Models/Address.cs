using AppCore.Enums;
using AppCore.ValueObjects;

namespace AppCore.Models;

public class Address : EntityBase
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public Country Country { get; set; } = new();
    public AddressType Type { get; set; }
}
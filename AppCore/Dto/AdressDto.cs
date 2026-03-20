using AppCore.Enums;

namespace AppCore.Dto;

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country,
    AddressType Type
);
using AppCore.Enums;
using AppCore.Models;

namespace AppCore.Dto;

public abstract record ContactBaseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public AddressDto? Address { get; init; }
    public ContactStatus Status { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    
    
    public static ContactBaseDto FromEntity(Contact contact)
    {
        
        throw new NotImplementedException();
    }
}
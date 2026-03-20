using AppCore.Enums;
using AppCore.Models;

namespace AppCore.Dto;

public record PersonDto : ContactBaseDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }
    public string? EmployerName { get; init; }
    
    // AJOUTEZ CETTE PROPRIÉTÉ
    public string FullName { get; init; } = string.Empty;

    // Méthode de mapping Entity -> DTO
    public static PersonDto FromEntity(Person person)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            FullName = $"{person.FirstName} {person.LastName}",  // Ajouté
            Email = person.Email,
            Phone = person.Phone,
            Position = person.Position,
            BirthDate = person.BirthDate,
            Gender = person.Gender,
            EmployerId = person.EmployerId,
            EmployerName = person.Employer?.Name,
            Status = person.Status,
            CreatedAt = person.CreatedAt,
            Address = person.Address != null ? new AddressDto(
                person.Address.Street,
                person.Address.City,
                person.Address.PostalCode,
                person.Address.Country?.Name ?? "",
                person.Address.Type
            ) : null,
            Tags = person.Tags.Select(t => t.Name).ToList()
        };
    }

    // Méthode de mapping DTO -> Entity (pour création)
    public static Person ToEntity(CreatePersonDto dto, Guid? id = null)
    {
        return new Person
        {
            Id = id ?? Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            EmployerId = dto.EmployerId,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    public static implicit operator PersonDto?(Person? person)
    {
        return person == null ? null : FromEntity(person);
    }
    
    public static explicit operator Person?(PersonDto? dto)
    {
        if (dto == null) return null;
        return new Person
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            EmployerId = dto.EmployerId,
            Status = dto.Status
        };
    }
}
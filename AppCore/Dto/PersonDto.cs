using AppCore.Enums;
using AppCore.Models;

namespace AppCore.Dto;

public record PersonDto : ContactBaseDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }
    public string? EmployerName { get; init; }
    public List<NoteDto> Notes { get; init; } = new();

    public static PersonDto FromEntity(Person person)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            FullName = $"{person.FirstName} {person.LastName}",
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
            Tags = person.Tags.Select(t => t.Name).ToList(),
            Notes = person.Notes.Select(n => new NoteDto
            {
                Id = n.Id,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                ContactId = n.ContactId
            }).ToList()
        };
    }
}
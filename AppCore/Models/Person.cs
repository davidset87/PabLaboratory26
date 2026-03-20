using System.Text.Json.Serialization;
using AppCore.Enums;

namespace AppCore.Models;

public class Person : Contact
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;
    
    public string? Position { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    
    public Guid? EmployerId { get; set; }
    public Company? Employer { get; set; }
    
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public override string GetDisplayName()
    {
        return $"{FirstName} {LastName}";
    }
}
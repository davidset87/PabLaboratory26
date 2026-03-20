using System.Text.Json.Serialization;
using AppCore.Enums;

namespace AppCore.Models;

public class Customer : Contact  // Important : hérite de Contact
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    public override string GetDisplayName()
    {
        return $"{FirstName} {LastName}";
    }
}
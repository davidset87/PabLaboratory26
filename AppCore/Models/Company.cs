namespace AppCore.Models;

public class Company : Organization
{
    public string? TaxId { get; set; } // NIP
    public string? RegistrationNumber { get; set; } // REGON
    public DateTime? FoundedDate { get; set; }
    public string? Website { get; set; }
}
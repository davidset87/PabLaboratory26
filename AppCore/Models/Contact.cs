using AppCore.Enums;

namespace AppCore.Models;

public abstract class Contact : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ContactStatus Status { get; set; } = ContactStatus.Active;
    
    public Guid? AddressId { get; set; }
    public Address? Address { get; set; }
    
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    
    public abstract string GetDisplayName();
}
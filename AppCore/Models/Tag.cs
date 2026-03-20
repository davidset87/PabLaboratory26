namespace AppCore.Models;

public class Tag : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
using AppCore.Enums;

namespace AppCore.Models;

public abstract class Organization : Contact
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public OrganizationType OrganizationType { get; set; }
    
    public ICollection<Person> Employees { get; set; } = new List<Person>();
    
    public ICollection<Person> Members { get; set; } = new List<Person>();
    
    public override string GetDisplayName()
    {
        return Name;
    }
}
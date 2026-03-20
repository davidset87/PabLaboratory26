namespace AppCore.Models;

public class Note : EntityBase
{
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
}
namespace AppCore.Dto;

public record NoteDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public Guid ContactId { get; init; }
}
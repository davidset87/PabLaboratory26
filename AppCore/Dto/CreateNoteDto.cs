namespace AppCore.Dto;

public record CreateNoteDto
{
    public string Content { get; init; } = string.Empty;
}
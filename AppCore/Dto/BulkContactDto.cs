namespace AppCore.Dto;

public class BulkContactDto
{
    public List<CreatePersonDto> Contacts { get; set; } = new();
    public DeduplicationConfigDto DeduplicationConfig { get; set; } = new();
}
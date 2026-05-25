using AppCore.Dto;

namespace AppCore.Interfaces;

public interface IPersonDeduplicationService
{
    Task<BulkOperationResultDto> DeduplicateAndAddContactsAsync(
        BulkContactDto bulkDto,
        string currentUserId,
        string currentUserEmail,
        string userRole);
    
    Task<List<PersonDto>> FindDuplicatesAsync(CreatePersonDto contact, DeduplicationConfigDto config);
}

public class BulkOperationResultDto
{
    public List<PersonDto> AddedContacts { get; set; } = new();
    public List<RemovedContactInfoDto> RemovedContacts { get; set; } = new();
    public int TotalProcessed { get; set; }
    public int AddedCount { get; set; }
    public int RemovedCount { get; set; }
}

public class RemovedContactInfoDto
{
    public Guid OriginalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
}
using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Repositories;

public interface IRemovedContactRepository : IGenericRepositoryAsync<RemovedContact>
{
    Task<List<RemovedContactDto>> GetByUserIdAsync(string userId);
    Task<List<RemovedContactDto>> GetByDateRangeAsync(DateTime from, DateTime to);
}

public class RemovedContactDto
{
    public Guid Id { get; set; }
    public string OriginalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RemovedByUserId { get; set; } = string.Empty;
    public string RemovedByUserEmail { get; set; } = string.Empty;
    public DateTime RemovedAt { get; set; }
    public string DeduplicationReason { get; set; } = string.Empty;
    public string DeduplicationStrategy { get; set; } = string.Empty;
}
using AppCore.Enums;

namespace AppCore.Models;

public class RemovedContact
{
    public Guid Id { get; set; }
    public string OriginalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string RemovedByUserId { get; set; } = string.Empty;
    public string RemovedByUserEmail { get; set; } = string.Empty;
    public DateTime RemovedAt { get; set; }
    public string DeduplicationReason { get; set; } = string.Empty;
    public string DeduplicationStrategy { get; set; } = string.Empty;
    public ContactStatus Status { get; set; }  
}
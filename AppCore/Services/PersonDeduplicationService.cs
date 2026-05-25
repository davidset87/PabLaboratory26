using AppCore.Dto;
using AppCore.Enums;
using AppCore.Helpers;
using AppCore.Interfaces;
using AppCore.Models;
using AppCore.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AppCore.Services;

public class PersonDeduplicationService : IPersonDeduplicationService
{
    private readonly IPersonRepository _personRepository;
    private readonly IRemovedContactRepository _removedContactRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PersonDeduplicationService> _logger;

    public PersonDeduplicationService(
        IPersonRepository personRepository,
        IRemovedContactRepository removedContactRepository,
        IMapper mapper,
        ILogger<PersonDeduplicationService> logger)
    {
        _personRepository = personRepository;
        _removedContactRepository = removedContactRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<PersonDto>> FindDuplicatesAsync(CreatePersonDto contact, DeduplicationConfigDto config)
    {
        var allPersons = await _personRepository.FindAllAsync();
        var duplicates = new List<Person>();

        foreach (var existing in allPersons)
        {
            if (IsDuplicate(existing, contact, config))
            {
                duplicates.Add(existing);
            }
        }

        return _mapper.Map<List<PersonDto>>(duplicates);
    }

    public async Task<BulkOperationResultDto> DeduplicateAndAddContactsAsync(
        BulkContactDto bulkDto,
        string currentUserId,
        string currentUserEmail,
        string userRole)
    {
        var result = new BulkOperationResultDto();
        var allowedRoles = new[] { "Administrator", "SalesManager" };

        if (!allowedRoles.Contains(userRole))
        {
            throw new UnauthorizedAccessException("Only Administrators and Sales Managers can perform deduplication.");
        }

        var existingContacts = (await _personRepository.FindAllAsync()).ToList();

        foreach (var newContact in bulkDto.Contacts)
        {
            result.TotalProcessed++;

            // Check for exact duplicate first
            var exactDuplicate = existingContacts.FirstOrDefault(e => IsExactDuplicate(e, newContact));
            
            if (exactDuplicate != null)
            {
                await HandleDuplicateAsync(
                    exactDuplicate, 
                    newContact, 
                    currentUserId, 
                    currentUserEmail, 
                    result, 
                    "Exact match - identical data",
                    "Exact");
                continue;
            }

            // Check for fuzzy duplicates
            var fuzzyDuplicate = existingContacts.FirstOrDefault(e => IsDuplicate(e, newContact, bulkDto.DeduplicationConfig));
            
            if (fuzzyDuplicate != null)
            {
                await HandleDuplicateAsync(
                    fuzzyDuplicate, 
                    newContact, 
                    currentUserId, 
                    currentUserEmail, 
                    result, 
                    $"Fuzzy match - similarity threshold {bulkDto.DeduplicationConfig.SimilarityThreshold}",
                    "Fuzzy (Jaro-Winkler)");
                continue;
            }

            // No duplicate found - add new contact
            var person = _mapper.Map<Person>(newContact);
            person.Id = Guid.NewGuid();
            person.CreatedAt = DateTime.UtcNow;
            person.UpdatedAt = DateTime.UtcNow;
            person.Status = ContactStatus.Active;

            var added = await _personRepository.AddAsync(person);
            result.AddedContacts.Add(_mapper.Map<PersonDto>(added));
            result.AddedCount++;
            
            existingContacts.Add(person);
        }

        return result;
    }

    private bool IsDuplicate(Person existing, CreatePersonDto newContact, DeduplicationConfigDto config)
    {
        if (config.PropertiesToCompare.Contains("FirstName") && config.PropertiesToCompare.Contains("LastName"))
        {
            var fullNameSimilar = JaroWinklerDistance.AreSimilar(
                $"{existing.FirstName} {existing.LastName}",
                $"{newContact.FirstName} {newContact.LastName}",
                config.SimilarityThreshold);
            
            if (fullNameSimilar) return true;
        }

        foreach (var prop in config.PropertiesToCompare)
        {
            switch (prop)
            {
                case "FirstName":
                    if (JaroWinklerDistance.AreSimilar(existing.FirstName, newContact.FirstName, config.SimilarityThreshold))
                        return true;
                    break;
                case "LastName":
                    if (JaroWinklerDistance.AreSimilar(existing.LastName, newContact.LastName, config.SimilarityThreshold))
                        return true;
                    break;
                case "Email":
                    if (string.Equals(existing.Email, newContact.Email, StringComparison.OrdinalIgnoreCase))
                        return true;
                    break;
                case "Phone":
                    if (string.Equals(existing.Phone, newContact.Phone, StringComparison.OrdinalIgnoreCase))
                        return true;
                    break;
            }
        }
        return false;
    }

    private bool IsExactDuplicate(Person existing, CreatePersonDto newContact)
    {
        return string.Equals(existing.FirstName, newContact.FirstName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existing.LastName, newContact.LastName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existing.Email, newContact.Email, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existing.Phone, newContact.Phone, StringComparison.OrdinalIgnoreCase);
    }

    private async Task HandleDuplicateAsync(
        Person existing,
        CreatePersonDto newContact,
        string userId,
        string userEmail,
        BulkOperationResultDto result,
        string reason,
        string strategy)
    {
        var removedContact = new RemovedContact
        {
            Id = Guid.NewGuid(),
            OriginalId = existing.Id.ToString(),
            FirstName = existing.FirstName,
            LastName = existing.LastName,
            Email = existing.Email,
            Phone = existing.Phone,
            Address = existing.Address?.ToString() ?? string.Empty,
            RemovedByUserId = userId,
            RemovedByUserEmail = userEmail,
            RemovedAt = DateTime.UtcNow,
            DeduplicationReason = reason,
            DeduplicationStrategy = strategy,
            Status = existing.Status
        };

        await _removedContactRepository.AddAsync(removedContact);
        
        result.RemovedContacts.Add(new RemovedContactInfoDto
        {
            OriginalId = existing.Id,
            Name = $"{existing.FirstName} {existing.LastName}",
            Email = existing.Email,
            Reason = reason,
            Strategy = strategy
        });
        result.RemovedCount++;
    }
}
using AppCore.Models;
using AppCore.Dto;
using AppCore.Repositories;
using AppCore.Enums;

namespace Infrastructure.Memory;

public class MemoryContactRepository : MemoryGenericRepository<Contact>, IContactRepository
{
    public async Task<PagedResult<Contact>> SearchAsync(ContactSearchDto searchDto)
    {
        var query = _data.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchDto.Query))
        {
            query = query.Where(c => 
                (c.Email?.Contains(searchDto.Query) ?? false) || 
                (c.Phone?.Contains(searchDto.Query) ?? false));
        }

        if (searchDto.Status.HasValue)
        {
            query = query.Where(c => c.Status == searchDto.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Tag))
        {
            query = query.Where(c => c.Tags.Any(t => t.Name == searchDto.Tag));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.ContactType))
        {
            query = query.Where(c => c.GetType().Name == searchDto.ContactType);
        }

        var items = query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();

        var result = new PagedResult<Contact>(
            items, 
            query.Count(), 
            searchDto.Page, 
            searchDto.PageSize);

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<Contact>> FindByTagAsync(string tag)
    {
        var result = _data.Values
            .Where(c => c.Tags.Any(t => t.Name == tag))
            .AsEnumerable();
        
        return await Task.FromResult(result);
    }

    public async Task<Note> AddNoteAsync(Guid contactId, string content)
    {
        if (!_data.TryGetValue(contactId, out var contact))
        {
            throw new KeyNotFoundException($"Contact with id {contactId} not found");
        }

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = content,
            ContactId = contactId,
            CreatedAt = DateTime.UtcNow
        };

        contact.Notes.Add(note);
        return await Task.FromResult(note);
    }

    public async Task<IEnumerable<Note>> GetNotesAsync(Guid contactId)
    {
        if (!_data.TryGetValue(contactId, out var contact))
        {
            throw new KeyNotFoundException($"Contact with id {contactId} not found");
        }

        return await Task.FromResult(contact.Notes.AsEnumerable());
    }

    public async Task AddTagAsync(Guid contactId, string tagName)
    {
        if (!_data.TryGetValue(contactId, out var contact))
        {
            throw new KeyNotFoundException($"Contact with id {contactId} not found");
        }

        var tag = new Tag { Id = Guid.NewGuid(), Name = tagName };
        contact.Tags.Add(tag);
        await Task.CompletedTask;
    }

    public async Task RemoveTagAsync(Guid contactId, string tagName)
    {
        if (!_data.TryGetValue(contactId, out var contact))
        {
            throw new KeyNotFoundException($"Contact with id {contactId} not found");
        }

        var tag = contact.Tags.FirstOrDefault(t => t.Name == tagName);
        if (tag != null)
        {
            contact.Tags.Remove(tag);
        }

        await Task.CompletedTask;
    }
}
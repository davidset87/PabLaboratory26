using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Repositories;

public interface IContactRepository : IGenericRepositoryAsync<Contact>
{
    Task<PagedResult<Contact>> SearchAsync(ContactSearchDto searchDto);
    Task<IEnumerable<Contact>> FindByTagAsync(string tag);
    Task<Note> AddNoteAsync(Guid contactId, string content);
    Task<IEnumerable<Note>> GetNotesAsync(Guid contactId);
    Task AddTagAsync(Guid contactId, string tagName);
    Task RemoveTagAsync(Guid contactId, string tagName);
}
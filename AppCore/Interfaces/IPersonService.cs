using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePagedAsync(int page, int pageSize);
    Task<IEnumerable<PersonDto>> FindPeopleFromCompanyAsync(Guid companyId);
    Task<PersonDto?> FindPersonByIdAsync(Guid id);
    Task<PersonDto> CreatePersonAsync(CreatePersonDto createDto);
    Task<PersonDto> UpdatePersonAsync(Guid id, UpdatePersonDto updateDto);
    Task DeletePersonAsync(Guid id);
    Task<IEnumerable<PersonDto>> SearchPeopleAsync(string query);
    Task AddNoteToPersonAsync(Guid personId, string content);
    Task<IEnumerable<Note>> GetPersonNotesAsync(Guid personId);
    Task AddTagToPersonAsync(Guid personId, string tagName);
    Task RemoveTagFromPersonAsync(Guid personId, string tagName);
}
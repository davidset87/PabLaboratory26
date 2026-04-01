using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePagedAsync(int page, int pageSize);
    Task<PersonDto?> FindPersonByIdAsync(Guid id);
    Task<IEnumerable<PersonDto>> FindPeopleFromCompanyAsync(Guid companyId);
    Task<IEnumerable<PersonDto>> FindPeopleFromOrganizationAsync(Guid organizationId);
    Task<IEnumerable<PersonDto>> SearchPeopleAsync(string query);
    Task<PersonDto> CreatePersonAsync(CreatePersonDto createDto);
    Task<PersonDto> UpdatePersonAsync(Guid id, UpdatePersonDto updateDto);
    Task DeletePersonAsync(Guid id);
    
    Task<Note> AddNoteToPersonAsync(Guid personId, CreateNoteDto noteDto);
    Task<PersonDto> GetPersonAsync(Guid personId);
    Task DeleteNoteAsync(Guid personId, Guid noteId);
}
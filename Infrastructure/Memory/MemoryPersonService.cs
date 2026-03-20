using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Enums;
using AutoMapper;

namespace Infrastructure.Memory;

public class MemoryPersonService : IPersonService
{
    private readonly IContactUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PersonDto>> FindAllPeoplePagedAsync(int page, int pageSize)
    {
        var people = await _unitOfWork.Persons.FindPagedAsync(page, pageSize);
        var items = _mapper.Map<List<PersonDto>>(people.Items);
        return new PagedResult<PersonDto>(items, people.TotalCount, people.Page, people.PageSize);
    }

    public async Task<PersonDto?> FindPersonByIdAsync(Guid id)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(id);
        return person == null ? null : _mapper.Map<PersonDto>(person);
    }

    public async Task<IEnumerable<PersonDto>> FindPeopleFromCompanyAsync(Guid companyId)
    {
        var people = await _unitOfWork.Persons.GetEmployeesOfCompanyAsync(companyId);
        return _mapper.Map<List<PersonDto>>(people);
    }

    public async Task<IEnumerable<PersonDto>> FindPeopleFromOrganizationAsync(Guid organizationId)
    {
        var people = await _unitOfWork.Persons.GetPeopleFromSameOrganizationAsync(organizationId);
        return _mapper.Map<List<PersonDto>>(people);
    }

    public async Task<IEnumerable<PersonDto>> SearchPeopleAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<PersonDto>();

        var allPeople = await _unitOfWork.Persons.FindAllAsync();
        var filtered = allPeople.Where(p => 
            p.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Email.Contains(query, StringComparison.OrdinalIgnoreCase));
        
        return _mapper.Map<List<PersonDto>>(filtered);
    }

    public async Task<PersonDto> CreatePersonAsync(CreatePersonDto createDto)
    {
        var person = _mapper.Map<Person>(createDto);
        person.Id = Guid.NewGuid();
        person.CreatedAt = DateTime.UtcNow;
        person.UpdatedAt = DateTime.UtcNow;
        person.Status = ContactStatus.Active;

        var added = await _unitOfWork.Persons.AddAsync(person);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PersonDto>(added);
    }

    public async Task<PersonDto> UpdatePersonAsync(Guid id, UpdatePersonDto updateDto)
    {
        var existing = await _unitOfWork.Persons.FindByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Person with id {id} not found");

        _mapper.Map(updateDto, existing);
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _unitOfWork.Persons.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PersonDto>(updated);
    }

    public async Task DeletePersonAsync(Guid id)
    {
        await _unitOfWork.Persons.RemoveByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddNoteToPersonAsync(Guid personId, string content)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = content,
            ContactId = personId,
            CreatedAt = DateTime.UtcNow
        };

        person.Notes.Add(note);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Note>> GetPersonNotesAsync(Guid personId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        return person.Notes;
    }

    public async Task AddTagToPersonAsync(Guid personId, string tagName)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = tagName
        };

        person.Tags.Add(tag);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveTagFromPersonAsync(Guid personId, string tagName)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        var tag = person.Tags.FirstOrDefault(t => t.Name == tagName);
        if (tag != null)
        {
            person.Tags.Remove(tag);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
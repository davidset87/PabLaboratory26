using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Enums;
using AutoMapper;

namespace AppCore.Services;

public class PersonService : IPersonService
{
    private readonly IContactUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PersonService(IContactUnitOfWork unitOfWork, IMapper mapper)
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

    public async Task<PersonDto> GetPersonAsync(Guid personId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        return _mapper.Map<PersonDto>(person);
    }

    public async Task<Note> AddNoteToPersonAsync(Guid personId, CreateNoteDto noteDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        if (person.Notes == null)
            person.Notes = new List<Note>();

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = noteDto.Content,
            ContactId = personId,
            CreatedAt = DateTime.UtcNow
        };

        person.Notes.Add(note);

        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();

        return note;
    }

    public async Task DeleteNoteAsync(Guid personId, Guid noteId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found");

        var note = person.Notes?.FirstOrDefault(n => n.Id == noteId);
        if (note == null)
            throw new KeyNotFoundException($"Note with id {noteId} not found");

        person.Notes.Remove(note);

        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();
    }
}
using AppCore.Authorization;
using AppCore.Dto;
using AppCore.Exceptions;
using AppCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly IPersonService _personService;

    public ContactsController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok("Contacts API is working");
    }

    [HttpGet("persons")]
    [Authorize(Policy = "ReadOnlyAccess")]
    public async Task<IActionResult> GetAllPersons([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _personService.FindAllPeoplePagedAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("persons/{id:guid}")]
    [Authorize(Policy = "ReadOnlyAccess")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        try
        {
            var person = await _personService.FindPersonByIdAsync(id);
            if (person == null)
                return NotFound(new { message = $"Person with id {id} not found" });
            
            return Ok(person);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("persons/search")]
    [Authorize(Policy = "ReadOnlyAccess")]
    public async Task<IActionResult> SearchPersons([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query parameter is required" });

            var results = await _personService.SearchPeopleAsync(query);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("companies/{companyId}/employees")]
    [Authorize(Policy = "SalesAccess")]
    public async Task<IActionResult> GetEmployeesByCompany(Guid companyId)
    {
        try
        {
            var employees = await _personService.FindPeopleFromCompanyAsync(companyId);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "SalesAccess")]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _personService.CreatePersonAsync(createDto);
            return CreatedAtAction(nameof(GetPersonById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "SalesManagerAccess")]
    public async Task<IActionResult> UpdatePerson(Guid id, [FromBody] UpdatePersonDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _personService.UpdatePersonAsync(id, updateDto);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Person with id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        try
        {
            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Person with id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{contactId:guid}/notes")]
    [Authorize(Policy = "SalesAccess")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNote(
        [FromRoute] Guid contactId,
        [FromBody] CreateNoteDto dto)
    {
        try
        {
            var note = await _personService.AddNoteToPersonAsync(contactId, dto);
            
            var noteDto = new NoteDto
            {
                Id = note.Id,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                ContactId = note.ContactId
            };
            
            return CreatedAtAction(nameof(GetNotes), new { contactId }, noteDto);
        }
        catch (ContactNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{contactId:guid}/notes")]
    [Authorize(Policy = "ReadOnlyAccess")]
    [ProducesResponseType(typeof(IEnumerable<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        try
        {
            var person = await _personService.GetPersonAsync(contactId);
            
            var notes = person.Notes.Select(n => new NoteDto
            {
                Id = n.Id,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                ContactId = n.ContactId
            });
            
            return Ok(notes);
        }
        catch (ContactNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{contactId:guid}/notes/{noteId:guid}")]
    [Authorize(Policy = "SalesManagerAccess")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteNote(
        [FromRoute] Guid contactId,
        [FromRoute] Guid noteId)
    {
        try
        {
            await _personService.DeleteNoteAsync(contactId, noteId);
            return NoContent();
        }
        catch (ContactNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
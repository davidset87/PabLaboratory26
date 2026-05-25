using System.Security.Claims;
using AppCore.Dto;
using AppCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/deduplication")]
[Authorize(Policy = "AdminOrSalesManager")]
public class DeduplicationController : ControllerBase
{
    private readonly IPersonDeduplicationService _deduplicationService;

    public DeduplicationController(IPersonDeduplicationService deduplicationService)
    {
        _deduplicationService = deduplicationService;
    }

    [HttpPost("bulk-add")]
    public async Task<IActionResult> BulkAddWithDeduplication([FromBody] BulkContactDto bulkDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "unknown";
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "ReadOnly";

            var result = await _deduplicationService.DeduplicateAndAddContactsAsync(
                bulkDto, userId, userEmail, userRole);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("find-duplicates")]
    public async Task<IActionResult> FindDuplicates(
        [FromBody] CreatePersonDto contact,
        [FromQuery] double threshold = 0.85)
    {
        try
        {
            var config = new DeduplicationConfigDto
            {
                SimilarityThreshold = threshold
            };
            
            var duplicates = await _deduplicationService.FindDuplicatesAsync(contact, config);
            return Ok(duplicates);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
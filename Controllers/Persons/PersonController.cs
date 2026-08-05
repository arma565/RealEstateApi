using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Persons;
using RealEstate.Services.Repositories.Persons;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Persons;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class PersonController(PersonRepository service, ILogger<PersonRepository> logger) : ControllerBase
{
    private readonly PersonRepository _service = service;

    private readonly ILogger<PersonRepository> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<Person>>> GetListAsync() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{personId}")]
    public async Task<ActionResult<Person>> GetAsync(string personId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(personId))
                return BadRequest("PersonId is empty!");

            if (!Guid.TryParse(personId, out Guid realEstatePersonid))
                return BadRequest("PersonId must be a valid GUID!");

            var person = await _service.GetAsync(realEstatePersonid).ConfigureAwait(false);

            return person == null ? NotFound() : Ok(person);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpGet("get-by-nationalId/{nationalId}")]
    public async Task<ActionResult<Person>> GetByNationalIdAsync(string nationalId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return BadRequest("NationalId is empty!");

            if (!long.TryParse(nationalId, out long realEstateNationalId))
                return BadRequest("NationalId must be long value!");

            var person = await _service.GetByNationalIdAsync(realEstateNationalId).ConfigureAwait(false);

            return person == null ? NotFound() : Ok(person);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] PersonDTO personDTO)
    {
        try
        {
            if (personDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          var person =  await _service.AddAsync(personDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { personId = person.Id }, person);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpPut("update/{personId}")]
    public async Task<IActionResult> UpdateAsync([FromBody] PersonDTO personDTO, string personId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(personId))
                return BadRequest("PersonId is empty!");

            if (!Guid.TryParse(personId, out Guid id))
                return BadRequest("PersonId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(personDTO , id).ConfigureAwait(false);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }

    }

    [HttpDelete("delete/{personId}")]
    public async Task<IActionResult> DeleteAsync(string personId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(personId))
                return BadRequest("PersonId is empty!");

            if (!Guid.TryParse(personId, out Guid id))
                return BadRequest("PersonId must be a valid GUID!");

            await _service.DeleteAsync(id).ConfigureAwait(false);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllAsync()
    {
        try
        {
            await _service.DeleteAllAsync().ConfigureAwait(false);
            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }

    }

}

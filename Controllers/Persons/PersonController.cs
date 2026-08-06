using Microsoft.AspNetCore.Mvc;
using RealEstate.Entities.Persons;
using RealEstate.Repositories.Persons;
using RealEstate.Services.Persons;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Persons;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class PersonController(PersonService service, ILogger<PersonRepository> logger) : ControllerBase
{
    private readonly PersonService _service = service;

    private readonly ILogger<PersonRepository> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<Person>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{personId}")]
    public async Task<ActionResult<Person>> Get(string personId)
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
    public async Task<ActionResult<Person>> GetByNationalId(string nationalId)
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
    public async Task<IActionResult> Add([FromBody] PersonDTO personDTO)
    {
        try
        {
            if (personDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          var person =  await _service.AddAsync(personDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { personId = person.Id }, person);
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
    public async Task<IActionResult> Update([FromBody] PersonDTO personDTO, string personId)
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
    public async Task<IActionResult> Delete(string personId)
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
    public async Task<IActionResult> DeleteAll()
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

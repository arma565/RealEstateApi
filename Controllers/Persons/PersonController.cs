using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Persons;
using RealEstate.Services.Repositories.Persons;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Persons;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class PersonController(PersonRepository service, ILogger logger) : ControllerBase
{
    private readonly PersonRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<IEnumerable<Person>> GetListAsync() => [.. await _service.GetListAsync().ConfigureAwait(false)];

    [HttpGet("/{personId}")]
    public async Task<ActionResult<Person>> GetAsync(string personId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(personId))
                return BadRequest("PersonId is empty!");

            if (!Guid.TryParse(personId, out Guid realEstatePersonid))
                return BadRequest("PersonId must be a valid GUID!");

            var person = await _service.GetByIdAsync(realEstatePersonid).ConfigureAwait(false);

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

    [HttpGet("national/{nationalId}")]
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

            var person = new Person
            {
                FirstName = personDTO.FirstName,
                LastName = personDTO.LastName,
                FatherName = personDTO.FatherName,
                BirthCertificateNumber = personDTO.BirthCertificateNumber,
                BirthCertificateIssued = personDTO.BirthCertificateIssued,
                NationalId = personDTO.NationalId,
                Born = personDTO.Born,
                Phone = personDTO.Phone,
                Address = personDTO.Address,
                Role = personDTO.Role,
                PropertyId = personDTO.PropertyId,
                LeaseId = personDTO.LeaseId
            };

            await _service.AddAsync(person).ConfigureAwait(false);

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

            if (!Guid.TryParse(personId, out Guid realEstatePersonid))
                return BadRequest("PersonId must be a valid GUID!");

            if (personDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPerson = await _service.IsPersonExistAsync(realEstatePersonid).ConfigureAwait(false);

            if (!existingPerson)
                return NotFound("Person not found!");

            var person = new Person
            {
                Id = realEstatePersonid,
                FirstName = personDTO.FirstName,
                LastName = personDTO.LastName,
                FatherName = personDTO.FatherName,
                BirthCertificateNumber = personDTO.BirthCertificateNumber,
                BirthCertificateIssued = personDTO.BirthCertificateIssued,
                NationalId = personDTO.NationalId,
                Born = personDTO.Born,
                Phone = personDTO.Phone,
                Address = personDTO.Address,
                Role = personDTO.Role,
                PropertyId = personDTO.PropertyId,
                LeaseId = personDTO.LeaseId
            };

            await _service.UpdateAsync(person).ConfigureAwait(false);

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

            if (!Guid.TryParse(personId, out Guid realEstatePersonid))
                return BadRequest("PersonId must be a valid GUID!");

            var existingPerson = await _service.IsPersonExistAsync(realEstatePersonid).ConfigureAwait(false);

            if (!existingPerson)
                return NotFound("Person not found!");

            await _service.DeleteAsync(realEstatePersonid).ConfigureAwait(false);

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

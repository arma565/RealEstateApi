using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Documents;
using RealEstate.Services.Repositories.Properties.Documents;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Documents;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class DeedController(DeedRepository service, ILogger logger) : ControllerBase
{
    private readonly DeedRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<IEnumerable<PropertyDeed>> GetListAsync() => [.. await _service.GetListAsync().ConfigureAwait(false)];

    [HttpGet("/{deedId}")]
    public async Task<ActionResult<PropertyDeed>> GetAsync(string deedId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(deedId))
                return BadRequest("DeedId is empty!");

            if (!Guid.TryParse(deedId, out Guid realEstateDeedId))
                return BadRequest("DeedId must be a valid GUID!");

            var deed = await _service.GetAsync(realEstateDeedId).ConfigureAwait(false);

            return deed == null ? NotFound() : Ok(deed);
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
    public async Task<IActionResult> AddAsync([FromBody] PropertyDeedDTO propertyDeedDTO)
    {
        try
        {
            if (propertyDeedDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deed = new PropertyDeed
            {
                DeedNumber = propertyDeedDTO.DeedNumber,
                RegistryNumber = propertyDeedDTO.RegistryNumber,
                IssueDate = propertyDeedDTO.IssueDate,
                IssuedBy = propertyDeedDTO.IssuedBy,
                ImageId = propertyDeedDTO.ImageId,
                PropertyId = propertyDeedDTO.PropertyId
            };

            await _service.AddAsync(deed).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { deedId = deed.Id }, deed);
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

    [HttpPut("update/{deedId}")]
    public async Task<IActionResult> UpdateAsync([FromBody] PropertyDeedDTO propertyDeedDTO, string deedId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(deedId))
                return BadRequest("DeedId is empty!");

            if (!Guid.TryParse(deedId, out Guid realEstateDeedId))
                return BadRequest("DeedId must be a valid GUID!");

            if (propertyDeedDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingDeed = await _service.IsDeedExistAsync(realEstateDeedId).ConfigureAwait(false);

            if (!existingDeed)
                return NotFound("Deed not found!");

            var deed = new PropertyDeed
            {
                Id = realEstateDeedId,
                DeedNumber = propertyDeedDTO.DeedNumber,
                RegistryNumber = propertyDeedDTO.RegistryNumber,
                IssueDate = propertyDeedDTO.IssueDate,
                IssuedBy = propertyDeedDTO.IssuedBy,
                ImageId = propertyDeedDTO.ImageId,
                PropertyId = propertyDeedDTO.PropertyId
            };

            await _service.UpdateAsync(deed).ConfigureAwait(false);

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

    [HttpDelete("delete/{deedId}")]
    public async Task<IActionResult> DeleteAsync(string deedId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(deedId))
                return BadRequest("DeedId is empty!");

            if (!Guid.TryParse(deedId, out Guid realEstateDeedId))
                return BadRequest("DeedId must be a valid GUID!");

            var existingDeed = await _service.IsDeedExistAsync(realEstateDeedId).ConfigureAwait(false);

            if (!existingDeed)
                return NotFound("Deed not found!");

            await _service.DeleteAsync(realEstateDeedId).ConfigureAwait(false);

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

using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Supports;
using RealEstate.Services.Repositories.Supports;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Supports;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public sealed class SupportController(SupportRepository service, ILogger logger) : ControllerBase
{
    private readonly SupportRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<ActionResult<IEnumerable<RealEstateSupport>>> GetListAsync() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("{supportId}")]
    public async Task<ActionResult<RealEstateSupport>> GetAsync(string supportId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportId))
                return BadRequest("SupportId is empty!");

            if (!Guid.TryParse(supportId, out Guid realEstateSupportId))
                return BadRequest("SupportId must be a valid GUID!");

            var support = await _service.GetAsync(realEstateSupportId).ConfigureAwait(false);

            return support == null ? NotFound() : Ok(support);

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
    public async Task<IActionResult> Add([FromBody] RealEstateSupportDTO realEstateSupportDTO)
    {
        try
        {
            if (realEstateSupportDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var realEstateSupport = new RealEstateSupport
            {
               Title = realEstateSupportDTO.Title,
               DetailsTitle = realEstateSupportDTO.DetailsTitle,
               DetailsSubtitle = realEstateSupportDTO.DetailsSubtitle,
               ImageId = realEstateSupportDTO.ImageId
            };

            await _service.AddAsync(realEstateSupport).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { supportId = realEstateSupport.Id }, realEstateSupport);
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

    [HttpPost("update/{supportId}")]
    public async Task<IActionResult> UpdateAsync([FromBody] RealEstateSupportDTO realEstateSupportDTO, string supportId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportId))
                return BadRequest("SupportId is empty!");

            if (!Guid.TryParse(supportId, out Guid realEstateSupportId))
                return BadRequest("SupportId must be a valid GUID!");

            if (realEstateSupportDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingSupport = await _service.IsSupportExistAsync(realEstateSupportId).ConfigureAwait(false);

            if (!existingSupport)
                return NotFound("Support not found!");

            var realEstateSupport = new RealEstateSupport
            {
                Id = realEstateSupportId,
                Title = realEstateSupportDTO.Title,
                DetailsTitle = realEstateSupportDTO.DetailsTitle,
                DetailsSubtitle = realEstateSupportDTO.DetailsSubtitle,
                ImageId = realEstateSupportDTO.ImageId
            };

            await _service.UpdateAsync(realEstateSupport).ConfigureAwait(false);

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

    [HttpDelete("delete/{supportId}")]
    public async Task<IActionResult> DeleteAsync(string supportId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportId))
                return BadRequest("SupportId is empty!");

            if (!Guid.TryParse(supportId, out Guid realEstateSupportId))
                return BadRequest("SupportId must be a valid GUID!");

            var existingSupport = await _service.IsSupportExistAsync(realEstateSupportId).ConfigureAwait(false);

            if (!existingSupport)
                return NotFound("Support not found!");

            await _service.DeleteAsync(realEstateSupportId).ConfigureAwait(false);

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
    public async Task<IActionResult> DeleteAllSupports()
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


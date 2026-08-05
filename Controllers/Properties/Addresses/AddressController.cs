using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Addresses;
using RealEstate.Services.Repositories.Properties.Addresses;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Addresses;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class AddressController(AddressRepository service, ILogger<AddressController> logger) : ControllerBase
{
    private readonly AddressRepository _service = service;

    private readonly ILogger<AddressController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<PropertyAddress>>> GetListAsync() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{addressId}")]
    public async Task<ActionResult<PropertyAddress>> GetAsync(string addressId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addressId))
                return BadRequest("AddressId is empty!");

            if (!Guid.TryParse(addressId, out Guid realEstateAddressId))
                return BadRequest("AddressId must be a valid GUID!");

            var address = await _service.GetAsync(realEstateAddressId).ConfigureAwait(false);

            return address == null ? NotFound() : Ok(address);
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
    public async Task<IActionResult> AddAsync([FromBody] PropertyAddressDTO propertyAddressDTO)
    {
        try
        {
            if (propertyAddressDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var address = await _service.AddAsync(propertyAddressDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { addressId = address.Id }, address);
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

    [HttpPut("update/{addressId}")]
    public async Task<IActionResult> UpdateAsync(string addressId, [FromBody] PropertyAddressDTO propertyAddressDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addressId))
                return BadRequest("AddressId is empty!");

            if (!Guid.TryParse(addressId, out Guid id))
                return BadRequest("AddressId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id , propertyAddressDTO).ConfigureAwait(false);

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

    [HttpDelete("delete/{addressId}")]
    public async Task<IActionResult> DeleteAsync(string addressId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addressId))
                return BadRequest("AddressId is empty!");

            if (!Guid.TryParse(addressId, out Guid realEstateAddressId))
                return BadRequest("AddressId must be a valid GUID!");

            await _service.DeleteAsync(realEstateAddressId).ConfigureAwait(false);

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

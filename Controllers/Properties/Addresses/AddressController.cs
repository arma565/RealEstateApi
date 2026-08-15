using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Properties.Addresses;
using RealEstate.Entities.Properties.Addresses;
using RealEstate.Services.Properties.Addresses;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Addresses;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class AddressController(AddressService service, ILogger<AddressController> logger) : ControllerBase
{
    private readonly AddressService _service = service;

    private readonly ILogger<AddressController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<PropertyAddress>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{addressId}")]
    public async Task<ActionResult<PropertyAddress>> Get(string addressId)
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
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CreateDTO propertyAddressDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(propertyAddressDTO , "PropertyAddressDTO is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var address = await _service.AddAsync(propertyAddressDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { addressId = address.Id.ToString() }, address);
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpPut("update/{addressId}")]
    public async Task<IActionResult> Update(string addressId, [FromBody] UpdateDTO propertyAddressDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addressId))
                return BadRequest("AddressId is empty!");

            if (!Guid.TryParse(addressId, out Guid id))
                return BadRequest("AddressId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id, propertyAddressDTO).ConfigureAwait(false);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpDelete("delete/{addressId}")]
    public async Task<IActionResult> Delete(string addressId)
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
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
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
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }
}

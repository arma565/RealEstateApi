using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Addresses;
using RealEstate.Services.Repositories.Properties.Addresses;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Addresses;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class AddressController(AddressRepository service, ILogger logger) : ControllerBase
{
    private readonly AddressRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<IEnumerable<PropertyAddress>> GetListAsync() => [.. await _service.GetListAsync().ConfigureAwait(false)];

    [HttpGet("/{addressId}")]
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

            var address = new PropertyAddress
            {
                Country = propertyAddressDTO.Country,
                Province = propertyAddressDTO.Province,
                City = propertyAddressDTO.City,
                District = propertyAddressDTO.District,
                Street = propertyAddressDTO.Street,
                PlatesNumber = propertyAddressDTO.PlatesNumber,
                PostalCode = propertyAddressDTO.PostalCode,
                PropertyId = propertyAddressDTO.PropertyId
            };

            await _service.AddAsync(address).ConfigureAwait(false);

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
    public async Task<IActionResult> UpdateAsync([FromBody] PropertyAddressDTO propertyAddressDTO, string addressId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addressId))
                return BadRequest("AddressId is empty!");

            if (!Guid.TryParse(addressId, out Guid realEstateAddressId))
                return BadRequest("AddressId must be a valid GUID!");

            if (propertyAddressDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAddress = await _service.IsAddressExistAsync(realEstateAddressId).ConfigureAwait(false);

            if (!existingAddress)
                return NotFound("Address not found!");

            var address = new PropertyAddress
            {
                Id = realEstateAddressId,
                Country = propertyAddressDTO.Country,
                Province = propertyAddressDTO.Province,
                City = propertyAddressDTO.City,
                District = propertyAddressDTO.District,
                Street = propertyAddressDTO.Street,
                PlatesNumber = propertyAddressDTO.PlatesNumber,
                PostalCode = propertyAddressDTO.PostalCode,
                PropertyId = propertyAddressDTO.PropertyId
            };

            await _service.UpdateAsync(address).ConfigureAwait(false);

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

            var existingAddress = await _service.IsAddressExistAsync(realEstateAddressId).ConfigureAwait(false);

            if (!existingAddress)
                return NotFound("Address not found!");

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
}

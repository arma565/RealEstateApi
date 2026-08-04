using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties;
using RealEstate.Services.Repositories.Properties;
using RealEstate.Services.Validations;
using System.Security;


namespace RealEstate.Controllers.Properties;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public sealed class PropertyController(PropertyRepository service, ILogger logger) : ControllerBase
{
    private readonly PropertyRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<ActionResult<IEnumerable<RealEstateProperty>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("/{propertyId}")]
    public async Task<ActionResult<RealEstateProperty>> GetAsync(string propertyId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                return BadRequest("PropertyId is empty!");

            if (!Guid.TryParse(propertyId, out Guid realEstatePropertyId))
                return BadRequest("PropertyId must be a valid GUID!");

            var property = await _service.GetAsync(realEstatePropertyId).ConfigureAwait(false);

            return property == null ? NotFound() : Ok(property);

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
    public async Task<IActionResult> AddAsync([FromBody] RealEstatePropertyDTO realEstatePropertyDTO)
    {
        try
        {
            if (realEstatePropertyDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool? isPropertyExists = await _service.IsPropertyExistAsync(realEstatePropertyDTO.PlatesNumber!).ConfigureAwait(false);

            if (isPropertyExists == true)
                return BadRequest("Property is already exist!");

            var lastProperty = await _service.LastProperty().ConfigureAwait(false);

            if (lastProperty == null)
                realEstatePropertyDTO.OrderId = 1;
            else
                realEstatePropertyDTO.OrderId++;

            var realEstateProperty = new RealEstateProperty
            {
                OrderId = realEstatePropertyDTO.OrderId,
                Title = realEstatePropertyDTO.Title,
                Description = realEstatePropertyDTO.Description,
                PlatesNumber = realEstatePropertyDTO.PlatesNumber,
                PropertyType = realEstatePropertyDTO.PropertyType,
                PropertyStatus = realEstatePropertyDTO.PropertyStatus,
                Price = realEstatePropertyDTO.Price,
                Currency = realEstatePropertyDTO.Currency,
                YearBuilt = realEstatePropertyDTO.YearBuilt,
                LandArea = realEstatePropertyDTO.LandArea,
                BuildingArea = realEstatePropertyDTO.BuildingArea,
                AddressId = realEstatePropertyDTO.AddressId,
                LocationId = realEstatePropertyDTO.LocationId,
                OwnerId = realEstatePropertyDTO.OwnerId,
                AgentId = realEstatePropertyDTO.AgentId,
                PropertyDeedId = realEstatePropertyDTO.PropertyDeedId,
                LeaseId = realEstatePropertyDTO.LeaseId
            };


            await _service.AddAsync(realEstateProperty).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { assetID = realEstateProperty.Id }, realEstateProperty);
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

    [HttpPut("update/{propertyId}")]
    public async Task<IActionResult> UpdateAsync([FromBody] RealEstatePropertyDTO realEstatePropertyDTO, string propertyId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                return BadRequest("PropertyId is empty!");

            if (!Guid.TryParse(propertyId, out Guid realEstatePropertyId))
                return BadRequest("PropertyId must be a valid GUID!");

            if (realEstatePropertyDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProperty = await _service.IsPropertyExistAsync(realEstatePropertyId).ConfigureAwait(false);

            if (!existingProperty)
                return NotFound("Property not found!");

            var property = new RealEstateProperty
            {
                Id = realEstatePropertyId,
                OrderId = realEstatePropertyDTO.OrderId,
                Title = realEstatePropertyDTO.Title,
                Description = realEstatePropertyDTO.Description,
                PlatesNumber = realEstatePropertyDTO.PlatesNumber,
                PropertyType = realEstatePropertyDTO.PropertyType,
                PropertyStatus = realEstatePropertyDTO.PropertyStatus,
                Price = realEstatePropertyDTO.Price,
                Currency = realEstatePropertyDTO.Currency,
                YearBuilt = realEstatePropertyDTO.YearBuilt,
                LandArea = realEstatePropertyDTO.LandArea,
                BuildingArea = realEstatePropertyDTO.BuildingArea,
                AddressId = realEstatePropertyDTO.AddressId,
                LocationId = realEstatePropertyDTO.LocationId,
                OwnerId = realEstatePropertyDTO.OwnerId,
                AgentId = realEstatePropertyDTO.AgentId,
                PropertyDeedId = realEstatePropertyDTO.PropertyDeedId,
                LeaseId = realEstatePropertyDTO.LeaseId
            };

            await _service.UpdateAsync(property).ConfigureAwait(false);

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

    [HttpDelete("delete/{propertyId}")]
    public async Task<IActionResult> DeleteAsync(string propertyId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                return BadRequest("PropertyId is empty!");

            if (!Guid.TryParse(propertyId, out Guid realEstatePropertyId))
                return BadRequest("PropertyId must be a valid GUID!");

            var existingProperty = await _service.IsPropertyExistAsync(realEstatePropertyId).ConfigureAwait(false);

            if (!existingProperty)
                return NotFound("Property not found!");

            await _service.DeleteAsync(realEstatePropertyId).ConfigureAwait(false);

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
    public async Task<IActionResult> DeleteAllAssets()
    {
        try
        {
            await _service.DeleteAllAsync().ConfigureAwait(false);
            return Ok("All assets has been deleted!");
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



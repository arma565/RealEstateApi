using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Models.Estate;
using RealEstate.Services;
using System.Globalization;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    internal sealed class EstateController(RepositoryService service) : ControllerBase
    {
        private readonly RepositoryService _service = service;

        #region "Property"
        [HttpGet("property")]
        public async Task<IEnumerable<Asset>> GetPropertyList() => await _service.GetPropertyList().ConfigureAwait(false);

        [HttpGet("property/{propertyID}")]
        public async Task<ActionResult<Asset>> GetProperty(Guid propertyID)
        {
            if (string.IsNullOrEmpty(propertyID.ToString()))
            {
                return BadRequest("Invalid propertyID");
            }
            Asset? property = await _service.GetProperty(propertyID).ConfigureAwait(false);
            if (property is null)
            {
                return NotFound("Property not found!");
            }
            else
            {
                return property;
            }
        }

        [HttpPost("property/add")]
        public async Task<IActionResult> AddProperty([FromBody] Asset newProperty)
        {
            if (newProperty == null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (newProperty.Date!.ToString().IsNullOrEmpty() && newProperty.Time.IsNullOrEmpty())
            {
                newProperty.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                newProperty.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            }
            bool? getPlatesNum = await _service.GetPropertyByPlateNumber(newProperty.PlatesNumber!).ConfigureAwait(false);
            if (getPlatesNum == true)
                return BadRequest("Plates number already exist!");
            var addedProperty = await _service.AddProperty(newProperty).ConfigureAwait(false);
            return CreatedAtAction(
                nameof(GetProperty),
                new { propertyID = addedProperty!.Id },
                addedProperty
            );
        }

        [HttpPut("property/update")]
        public async Task<IActionResult> UpdateProperty([FromBody] Asset updateProperty)
        {
            if (updateProperty == null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (string.IsNullOrEmpty(updateProperty.Id.ToString()))
            {
                return BadRequest("Updating property is not possible without id!");
            }
            Asset? property = await _service.GetProperty(updateProperty.Id).ConfigureAwait(false);
            if (property is null)
            {
                return NotFound("Property not found!");
            }
            updateProperty.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            updateProperty.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _service.UpdateProperty(updateProperty).ConfigureAwait(false);
            return NoContent();
        }

        [HttpDelete("property/delete/{id}")]
        public async Task<IActionResult> DeleteProperty(Guid id)
        {
            Asset? property = await _service.GetProperty(id).ConfigureAwait(false);
            if (property is null)
            {
                return NotFound();
            }
            else
            {
                _service.DeleteProperty(property);
                return Ok();
            }
        }

        [HttpDelete("property/delete-all")]
        public IActionResult DeleteProperties()
        {
            _service.DeleteAllProperties();
            return Ok();
        }
        #endregion

        #region "Person"
        [HttpGet("persons")]
        public async Task<IEnumerable<Person>> GetPersonsList() => await _service.GetPersonsList().ConfigureAwait(false);

        [HttpGet("person/{id}")]
        public async Task<ActionResult<Person>> GetPerson(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person? person = await _service.GetPerson(id).ConfigureAwait(false);
            if (person is null)
            {
                return NotFound("Person not found!");
            }
            else
            {
                return person;
            }
        }

        [HttpPost("person/add")]
        public async Task<IActionResult> AddPerson([FromBody] Person newPerson)
        {
            if (newPerson == null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (newPerson.PropertyID.ToString().IsNullOrEmpty()) { 
                return BadRequest("PropertyID can't be empty");
            }
            var existProperty = await _service.GetProperty(newPerson.PropertyID).ConfigureAwait(false);
            if (existProperty is null)
                return NotFound("PropertyID is incorrect or property not found!");
            var addedPerson = await _service.AddPerson(newPerson).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetPerson), new { id = addedPerson.Id }, addedPerson);
        }

        [HttpPut("person/update")]
        public async Task<IActionResult> UpdatePerson([FromBody] Person updatePerson)
        {
            if (updatePerson == null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (updatePerson.PropertyID.ToString().IsNullOrEmpty()) { 
                return BadRequest("PropertyID can't be empty");
            }
            var existProperty = await _service.GetProperty(updatePerson.PropertyID).ConfigureAwait(false);
            if (existProperty is null)
                return NotFound("PropertyID is incorrect or property not found!");
            Person? existPerson = await _service.GetPerson(updatePerson.Id).ConfigureAwait(false);
            if (existPerson is null)
            {
                return NotFound("Person not found!");
            }
            else
            {
                var updatedPerson = await _service.UpdatePerson(updatePerson).ConfigureAwait(false);
                return Ok(updatedPerson);
            }
        }

        [HttpDelete("person/delete/{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person? person = await _service.GetPerson(id).ConfigureAwait(false);
            if (person is null)
            {
                return NotFound("Person not found!");
            }
            else
            {
                _service.DeletePerson(person);
                return Ok();
            }
        }

        [HttpDelete("person/delete-all")]
        public IActionResult DeleteAllPersons()
        {
            _service.DeleteAllPersons();
            return Ok();
        }
        #endregion
    }
}


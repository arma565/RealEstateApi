using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("[controller]")]
public class EstateController : ControllerBase
{
    private readonly RepositoryService _service;

    public EstateController(RepositoryService service)
    {
        _service = service;
    }

    #region "Property"
    [HttpGet("property")]
    public async Task<IEnumerable<Property>> GetPropertyList() => await _service.GetPropertyList();

    [HttpGet("property/{propertyID}")]
    public async Task<ActionResult<Property>> GetProperty(int propertyID)
    {
        if (propertyID <= 0)
        {
            return BadRequest("Invalid propertyID");
        }
        Property? property = await _service.GetProperty(propertyID);
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
    public async Task<IActionResult> AddProperty([FromBody] Property newProperty)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (newProperty.Date!.ToString().IsNullOrEmpty() && newProperty.Time.IsNullOrEmpty())
        {
            newProperty.Date = DateTime.Now.ToString("yyyy-MM-dd");
            newProperty.Time = DateTime.Now.ToString("HH:mm:ss");
        }
        bool? getPlatesNum = await _service.GetPropertyByPlateNumber(newProperty.PlatesNumber!);
        if (getPlatesNum == true)
            return BadRequest("Plates number already exist!");
        var addedProperty = await _service.AddProperty(newProperty);
        return CreatedAtAction(
            nameof(GetProperty),
            new { propertyID = addedProperty!.Id },
            addedProperty
        );
    }

    [HttpPut("property/update")]
    public async Task<IActionResult> UpdateProperty([FromBody] Property updateProperty)
    {
        if (updateProperty.Id <= 0)
        {
            return BadRequest("Updating property is not possible without id!");
        }
        Property? property = await _service.GetProperty(updateProperty.Id);
        if (property is null)
        {
            return NotFound("Property not found!");
        }
        updateProperty.Date = DateTime.Now.ToString("yyyy-MM-dd");
        updateProperty.Time = DateTime.Now.ToString("HH:mm:ss");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _service.UpdateProperty(updateProperty);
        return NoContent();
    }

    [HttpDelete("property/delete/{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        Property? property = await _service.GetProperty(id);
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
    public async Task<IEnumerable<Person>> GetPersonsList() => await _service.GetPersonsList();

    [HttpGet("person/{id}")]
    public async Task<ActionResult<Person>> GetPerson(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Person? person = await _service.GetPerson(id);
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (newPerson.PropertyID.ToString().IsNullOrEmpty())
            return BadRequest("PropertyID can't be empty");
        var existProperty = await _service.GetProperty(newPerson.PropertyID);
        if (existProperty is null)
            return NotFound("PropertyID is incorrect or property not found!");
        var addedPerson = await _service.AddPerson(newPerson);
        return CreatedAtAction(nameof(GetPerson), new { id = addedPerson.Id }, addedPerson);
    }

    [HttpPut("person/update")]
    public async Task<IActionResult> UpdatePerson([FromBody] Person updatePerson)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (updatePerson.PropertyID.ToString().IsNullOrEmpty())
            return BadRequest("PropertyID can't be empty");
        var existProperty = await _service.GetProperty(updatePerson.PropertyID);
        if (existProperty is null)
            return NotFound("PropertyID is incorrect or property not found!");
        Person? existPerson = await _service.GetPerson(updatePerson.Id);
        if (existPerson is null)
        {
            return NotFound("Person not found!");
        }
        else
        {
            var updatedPerson = await _service.UpdatePerson(updatePerson);
            return Ok(updatedPerson);
        }
    }

    [HttpDelete("person/delete/{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Person? person = await _service.GetPerson(id);
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

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

    #region
    [HttpGet("/property")]
    public async Task<IEnumerable<Property>> GetPropertyList() => await _service.GetPropertyList();

    [HttpGet("/property/{propertyID}")]
    public async Task<ActionResult<Property>> GetProperty(int propertyID)
    {
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

    [HttpPost("/property/add")]
    public async Task<IActionResult> AddProperty(Property newProperty)
    {
        if (newProperty.PlatesNumber is null or "") return BadRequest("Plates number can't be empty!");
        bool? getPlatesNum = await _service.GetPropertyByPlateNumber(newProperty.PlatesNumber);
        if (getPlatesNum == true) return BadRequest("Plates number already exist!");
        var addedProperty = await _service.AddProperty(newProperty);
        return CreatedAtAction(nameof(GetProperty), new { propertyID = addedProperty!.Id }, addedProperty); ;
    }

    [HttpPut("/property/update/")]
    public async Task<IActionResult> UpdateProperty(Property updateProperty)
    {
        Property? property = await _service.GetProperty(updateProperty.Id);
        if (property is null)
        {
            return NotFound("Property not found!");
        }
        else
        {
            _service.UpdateProperty(updateProperty);
            return NoContent();
        }

    }

    [HttpDelete("/property/delete/{id}")]
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

    [HttpDelete("/property/delete-all")]
    public IActionResult DeleteProperties()
    {
        _service.DeleteAllProperties();
        return Ok();
    }
    #endregion

    #region 
    [HttpGet("/person/{id}")]
    public async Task<ActionResult<Person>> GetPerson(int id)
    {
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

    [HttpPost("/person/add")]
    public async Task<IActionResult> AddPerson(Person newPerson)
    {
        if (newPerson.PropertyID.ToString().IsNullOrEmpty()) return BadRequest("PropertyID can't be empty");
        var existProperty = await _service.GetProperty(newPerson.PropertyID);
        if (existProperty is null) return NotFound("PropertyID is incorrect or property not found!");
        var addedPerson = await _service.AddPerson(newPerson);
        return CreatedAtAction(nameof(GetPerson), new { id = addedPerson.Id }, addedPerson);
    }

    [HttpPut("/person/update/")]
    public async Task<IActionResult> UpdatePerson(Person updatePerson)
    {
        if (updatePerson.PropertyID.ToString().IsNullOrEmpty()) return BadRequest("PropertyID can't be empty");
        var existProperty = await _service.GetProperty(updatePerson.PropertyID);
        if (existProperty is null) return NotFound("PropertyID is incorrect or property not found!");
        Person? existPerson = await _service.GetPerson(updatePerson.Id);
        if (existPerson is null)
        {
            return NotFound("Person not found!");
        }
        else
        {
            _service.UpdatePerson(updatePerson);
            return NoContent();
        }
    }

    [HttpDelete("/person/delete/{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
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

    [HttpDelete("/person/delete-all")]
    public IActionResult DeleteAllPersons()
    {
        _service.DeleteAllPersons();
        return Ok();
    }
    #endregion
}
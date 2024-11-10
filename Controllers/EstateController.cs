using Microsoft.AspNetCore.Mvc;

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
    public IEnumerable<Property> GetPropertyList() => _service.GetPropertyList();

    [HttpGet("/property/{propertyID}")]
    public ActionResult<Property> GetProperty(int propertyID)
    {
        Property? property = _service.GetProperty(propertyID);
        if (property is null)
        {
            return NotFound();
        }
        else
        {
            return property;
        }
    }

    [HttpPost("/property/add")]
    public IActionResult AddProperty(Property newProperty)
    {
        if (newProperty.PlatesNumber is null or "" || _service.GetPropertyByPlateNumber(newProperty.PlatesNumber)) return BadRequest();
        var addedProperty = _service.AddProperty(newProperty);
        var response = CreatedAtAction(nameof(GetProperty), new { propertyID = addedProperty!.Id }, addedProperty);
        Console.WriteLine("Response =>" + response);
        return response;
    }

    [HttpPut("/property/update/")]
    public IActionResult UpdateProperty(Property updateProperty)
    {
        Property? property = _service.GetProperty(updateProperty.Id);
        if (property is null)
        {
            return NotFound();
        }
        else
        {   
            _service.UpdateProperty(updateProperty);
            return NoContent();
        }

    }

    [HttpDelete("/property/delete/")]
    public IActionResult DeleteProperty(Property propertyInput)
    {
        Property? property = _service.GetProperty(propertyInput.Id);
        if (property is null)
        {
            return NotFound();
        }
        else
        {
            _service.DeleteProperty(propertyInput);
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
    public ActionResult<Person> GetPerson(int id)
    {
        Person? person = _service.GetPerson(id);
        if (person is null)
        {
            return NotFound();
        }
        else
        {
            return person;
        }
    }
    
    [HttpPost("/person/add")]
    public IActionResult AddPerson(Person newPerson)
    {
        var addedPerson = _service.AddPerson(newPerson);
        return CreatedAtAction(nameof(GetPerson), new { id = addedPerson.Id }, addedPerson);
    }

    [HttpPut("/person/update/")]
    public IActionResult UpdatePerson(Person updatePerson)
    {
        Person? existPerson = _service.GetPerson(updatePerson.Id);
        if (existPerson is null)
        {
            return NotFound();
        }
        else
        {
            _service.UpdatePerson(updatePerson);
            return NoContent();
        }
    }

    [HttpDelete("/person/delete/")]
    public IActionResult DeletePerson(Person inputPerson)
    {
        Person? existPerson = _service.GetPerson(inputPerson.Id);
        if (existPerson is null)
        {
            return NotFound();
        }
        else
        {
            _service.DeletePerson(existPerson);
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
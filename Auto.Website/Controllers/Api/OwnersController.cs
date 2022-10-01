using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Controllers.Api;



[Route("api/[controller]")]
[ApiController]
public class OwnersController:ControllerBase
{
    private readonly IAutoDatabase db;

    public OwnersController(IAutoDatabase db)
    {
        this.db = db;
    }

    [HttpGet]
    [Produces("application/hal+json")]
    public IActionResult Get(int index = 0, int count = 10)
    {
        var items = db.ListOwners().Skip(index).Take(count);
        var total =db.CountOwners();
        var links = Paginate("/api/owners", index, count, total);
        
        var result = new
        {
            links, index, count, total, items
        };
        return Ok(result);
    }

    [HttpGet("{surname}")]
    public IActionResult Get(string surname)
    {
        var owner = db.FindOwnerBySurname(surname);
        if (owner == default) return NotFound();
        
        var json = owner.ToDynamic();
        json._links = new {
            self = new { href = $"/api/owners/{surname}" },
            vehicle = new { href = $"/api/vehicle/{owner.VehicleOfOwner.Registration}" }
        };
        json._actions = new {
            update = new {
                method = "PUT",
                href = $"/api/owners/{surname}",
                accept = "application/json"
            },
            delete = new {
                method = "DELETE",
                href = $"/api/owners/{surname}"
            }
        };
        return Ok(json);
    }
    
    
    
    
    [HttpDelete("{surname}")]
    public IActionResult Remove(string surname)
    {
        var owner = db.FindOwnerBySurname(surname);
        db.DeleteOwner(owner);
        return Ok(owner);
    }
    
    [HttpPut("{surname}")]
    public IActionResult Put(string surname, [FromBody] OwnerDto owner) {
        
        var ownerVehicle = db.FindVehicle(owner.VehicleOfOwner);

        var newOwner = new Owner
        {   
            Name = owner.Name,
            Surname = surname,
            PhoneNumber = owner.PhoneNumber,
            VehicleOfOwner = ownerVehicle
        };
        
        db.UpdateOwner(newOwner);
        
        return Get(newOwner.Surname);
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] OwnerDto dto)
    {

        var newVehicle = db.FindVehicle(dto.VehicleOfOwner);
        
        var newOwner = new Owner {
            Name = dto.Name,
            Surname = dto.Surname,
            PhoneNumber = dto.PhoneNumber,
            VehicleOfOwner = newVehicle
        };
        db.CreateOwner(newOwner);
			
        return Ok(dto);
    }
    
    private dynamic Paginate(string url, int index, int count, int total)
    {
        dynamic links = new ExpandoObject();
        links.self = new {href = url};
        links.final = new {href = $"{url}?index={total - (total % count)}&count={count}"};
        links.first = new {href = $"{url}?index=0&count={count}"};
        if (index > 0) links.previous = new {href = $"{url}?index={index - count}&count={count}"};
        if (index + count < total) links.next = new {href = $"{url}?index={index + count}&count={count}"};
        return links;
    }
    
    
    
}
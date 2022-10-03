using System;
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
    private readonly IAutoDatabase _db;

    public OwnersController(IAutoDatabase db)
    {
        _db = db;
    }

    [HttpGet]
    [Produces("application/hal+json")]
    public async Task<IActionResult> Get(int index = 0, int count = 10)
    {
        try
        {
            var items = _db.ListOwners().Skip(index).Take(count);
            var total =_db.CountOwners();
            var links = Paginate("/api/owners", index, count, total);
        
            var result = new
            {
                links, index, count, total, items
            };
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }

    [HttpGet("{surname}")]
    public async Task<IActionResult> Get(string surname)
    {
        try
        {
            var owner = _db.FindOwnerBySurname(surname);
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
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    
    
    
    [HttpDelete("{surname}")]
    public async Task<IActionResult> Remove(string surname)
    {
        try
        {
            var owner = _db.FindOwnerBySurname(surname);
            _db.DeleteOwner(owner);
            return Ok(owner);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{surname}")]
    public async Task<IActionResult> Put(string surname, [FromBody] OwnerDto owner) 
    {
        try
        {
            var ownerVehicle = _db.FindVehicle(owner.VehicleOfOwner);

            var newOwner = new Owner
            {   
                Name = owner.Name,
                Surname = surname,
                PhoneNumber = owner.PhoneNumber,
                VehicleOfOwner = ownerVehicle
            };
        
            _db.UpdateOwner(newOwner);
            
            var json = newOwner.ToDynamic();
            json._links = new {
                self = new { href = $"/api/owners/{newOwner.Surname}" },
                vehicle = new { href = $"/api/vehicle/{newOwner.VehicleOfOwner.Registration}" }
            };
            json._actions = new {
                update = new {
                    method = "PUT",
                    href = $"/api/owners/{newOwner.Surname}",
                    accept = "application/json"
                },
                delete = new {
                    method = "DELETE",
                    href = $"/api/owners/{newOwner.Surname}"
                }
            };
        
            return Ok(json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] OwnerDto dto)
    {
        try
        {
            var newVehicle = _db.FindVehicle(dto.VehicleOfOwner);
        
            var newOwner = new Owner {
                Name = dto.Name,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                VehicleOfOwner = newVehicle
            };
            _db.CreateOwner(newOwner);
			
            var json = newOwner.ToDynamic();
            json._links = new {
                self = new { href = $"/api/owners/{newOwner.Surname}" },
                vehicle = new { href = $"/api/vehicle/{newOwner.VehicleOfOwner.Registration}" }
            };
            json._actions = new {
                update = new {
                    method = "PUT",
                    href = $"/api/owners/{newOwner.Surname}",
                    accept = "application/json"
                },
                delete = new {
                    method = "DELETE",
                    href = $"/api/owners/{newOwner.Surname}"
                }
            };
            
            return Ok(json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
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
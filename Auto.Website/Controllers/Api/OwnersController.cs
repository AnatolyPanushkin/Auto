using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.Models;
using AutoMessages;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Controllers.Api;



[Route("api/[controller]")]
[ApiController]
public class OwnersController:ControllerBase
{
    private readonly IAutoDatabase _db;
    private readonly IBus _bus;

    public OwnersController(IAutoDatabase db,IBus bus)
    {
        _db = db;
        _bus = bus;
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

    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email)
    {
        try
        {
            var owner = _db.FindOwnerByEmail(email);
            if (owner == default) return NotFound();
        
            var json = owner.ToDynamic();
            json._links = new {
                self = new { href = $"/api/owners/{email}" },
                vehicle = new { href = $"/api/vehicle/{owner.VehicleOfOwner.Registration}" }
            };
            json._actions = new {
                update = new {
                    method = "PUT",
                    href = $"/api/owners/{email}",
                    accept = "application/json"
                },
                delete = new {
                    method = "DELETE",
                    href = $"/api/owners/{email}"
                }
            };
            return Ok(json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    
    
    
    [HttpDelete("{email}")]
    public async Task<IActionResult> Remove(string email)
    {
        try
        {
            var owner = _db.FindOwnerByEmail(email);
            _db.DeleteOwner(owner);
            return Ok(owner);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{email}")]
    public async Task<IActionResult> Put(string email, [FromBody] OwnerDto owner) 
    {
        try
        {
            var ownerVehicle = _db.FindVehicle(owner.VehicleOfOwner);

            var newOwner = new Owner
            {   
                Name = owner.Name,
                Surname = owner.Surname,
                PhoneNumber = owner.PhoneNumber,
                Email = owner.Email,
                VehicleOfOwner = ownerVehicle
            };
        
            _db.UpdateOwner(email,newOwner);
            
            var json = newOwner.ToDynamic();
            json._links = new {
                self = new { href = $"/api/owners/{newOwner.Email}" },
                vehicle = new { href = $"/api/vehicle/{newOwner.VehicleOfOwner.Registration}" }
            };
            json._actions = new {
                update = new {
                    method = "PUT",
                    href = $"/api/owners/{newOwner.Email}",
                    accept = "application/json"
                },
                delete = new {
                    method = "DELETE",
                    href = $"/api/owners/{newOwner.Email}"
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
                Email = dto.Email,
                VehicleOfOwner = newVehicle
            };
            _db.CreateOwner(newOwner);
            PublishNewOwnerMessage(newOwner);
			
            var json = newOwner.ToDynamic();
            json._links = new {
                self = new { href = $"/api/owners/{newOwner.Email}" },
                vehicle = new { href = $"/api/vehicle/{newOwner.VehicleOfOwner.Registration}" }
            };
            json._actions = new {
                update = new {
                    method = "PUT",
                    href = $"/api/owners/{newOwner.Email}",
                    accept = "application/json"
                },
                delete = new {
                    method = "DELETE",
                    href = $"/api/owners/{newOwner.Email}"
                }
            };
            
            return Ok(json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }

    private void PublishNewOwnerMessage(Owner owner)
    {
        var message = new NewOwnerMessage()
        {
            Name = owner.Name,
            Surname = owner.Surname,
            PhoneNumber = owner.PhoneNumber,
            Email = owner.Email,
            VehicleOfOwner = owner.VehicleOfOwner.Registration,
            ListedAtUtc = DateTime.UtcNow
        };
        _bus.PubSub.Publish(message);
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
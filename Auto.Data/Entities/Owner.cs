using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Auto.Data.Entities;

public partial class Owner
{
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string Email { get; set; }
    
    [JsonIgnore]
    public virtual Vehicle VehicleOfOwner{ get; set; }
}
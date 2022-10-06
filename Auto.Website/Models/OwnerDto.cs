namespace Auto.Website.Models;

public class OwnerDto
{
    public OwnerDto()
    {
    }
    public OwnerDto(string Name, string? Surname, string PhoneNumber,string Email, string VehicleOfOwner = null)
    {
        this.Name = Name;
        this.Surname = Surname;
        this.PhoneNumber= PhoneNumber;
        this.Email = Email;
        this.VehicleOfOwner = VehicleOfOwner;
    }
    public string Name { get; set; }
    public string Surname { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }
    public string? VehicleOfOwner { get; set; }
}
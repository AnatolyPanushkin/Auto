namespace AutoMessages;

public class NewOwnerMessage
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string VehicleOfOwner { get; set; }
    public DateTime ListedAtUtc { get; set; }
}
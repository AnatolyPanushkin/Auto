using Auto.Data.Entities;
using GraphQL.Types;

namespace Auto.Website.GraphQL.GraphTypes;

public class OwnerGraphType:ObjectGraphType<Owner>
{
    public OwnerGraphType() {
        Name = "owner";
        Field(c => c.Name, nullable:false).Description("Name of owner!");
        Field(c => c.Surname, nullable:false).Description("Surname of owner!");
        Field(c => c.PhoneNumber, nullable:false).Description("The phone number of owner!");
        Field(c => c.VehicleOfOwner, nullable: false, type: typeof(VehicleGraphType)).Description("Vehicle of owner!");
    }
}
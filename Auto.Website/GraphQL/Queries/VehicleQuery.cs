using Auto.Data;
using Auto.Data.Entities;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class VehicleQuery: ObjectGraphType
{
    private readonly IAutoDatabase db;

    public VehicleQuery(IAutoDatabase db)
    {
        this.db = db;
    }
    private Vehicle GetVehicle(IResolveFieldContext<object> context)
    {
        var registration = context.GetArgument<string>("registration");
        return db.FindVehicle(registration);
    }
    
}
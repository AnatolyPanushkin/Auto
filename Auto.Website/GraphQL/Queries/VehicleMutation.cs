using System;
using Auto.Data;
using Auto.Data.Entities;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class VehicleMutation : ObjectGraphType
{
    private readonly IAutoDatabase db;

    public VehicleMutation(IAutoDatabase db)
    {
        this.db = db;
    }
}
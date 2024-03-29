﻿using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class OwnerQuery: ObjectGraphType
{
    private readonly IAutoDatabase db;

    public OwnerQuery(IAutoDatabase context)
    {
        this.db = context;
        Field<OwnerGraphType>("owner", "Запрос для получения данных о владельце",
            new QueryArguments(MakeNonNullStringArgument("name", "Полное имя искомого владельца")),
            resolve: GetOwner);
    }
    
    private Owner GetOwner(IResolveFieldContext<object> context)
    {
        var name = context.GetArgument<string>("name");
        return db.FindOwnerBySurname(name);
    }
    
    private QueryArgument MakeNonNullStringArgument(string name, string description) {
        return new QueryArgument<NonNullGraphType<StringGraphType>> {
            Name = name, Description = description
        };
    }
}
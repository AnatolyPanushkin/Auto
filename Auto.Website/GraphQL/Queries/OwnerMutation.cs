using System;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class OwnerMutation: ObjectGraphType
{
    private readonly IAutoDatabase db;

    public OwnerMutation(IAutoDatabase db)
    {
        this.db = db;

        Field<OwnerGraphType>("createOwner", "Создание нового владельца", arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Name"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Surname"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "PhoneNumber"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Registration"}
            ),
            resolve: ownerContex =>
            {
                var name = ownerContex.GetArgument<string>("Name");
                var surname = ownerContex.GetArgument<string>("Surname");
                var phoneNumber = ownerContex.GetArgument<string>("PhoneNumber");
                var registration = ownerContex.GetArgument<string>("Registration");

                var newVehicle = db.FindVehicle(registration);

                var newOwner = new Owner()
                {
                    Name = name,
                    Surname = surname,
                    PhoneNumber = phoneNumber,
                    VehicleOfOwner = newVehicle
                };
                
                db.CreateOwner(newOwner);
                
                return new Owner
                {
                    Name = name,
                    Surname = surname
                };
            });

        Field<OwnerGraphType>("updateOwner", "Обновление владельца", arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Name"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Surname"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "PhoneNumber"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Registration"}
            ),resolve: ownerContex =>
            {
                var name = ownerContex.GetArgument<string>("Name");
                var surname = ownerContex.GetArgument<string>("Surname");
                var phoneNumber = ownerContex.GetArgument<string>("PhoneNumber");
                var registration = ownerContex.GetArgument<string>("Registration");
                
                var newVehicle = db.FindVehicle(registration);

                var updateOwner = new Owner()
                {
                    Name = name,
                    Surname = surname,
                    PhoneNumber = phoneNumber,
                    VehicleOfOwner = newVehicle
                };
                
                db.UpdateOwner(updateOwner);
                
                
                return updateOwner;
            });
        
        Field<OwnerGraphType>("deleteOwner", "Удаление владельца", arguments: new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Name"},
            new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Surname"},
            new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "PhoneNumber"},
            new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "Registration"}
        ),resolve: ownerContex =>
        {
            var name = ownerContex.GetArgument<string>("Name");
            var surname = ownerContex.GetArgument<string>("Surname");
            var phoneNumber = ownerContex.GetArgument<string>("PhoneNumber");
            var registration = ownerContex.GetArgument<string>("Registration");
                
            var newVehicle = db.FindVehicle(registration);

            var deleteOwner = new Owner()
            {
                Name = name,
                Surname = surname,
                PhoneNumber = phoneNumber,
                VehicleOfOwner = newVehicle
            };
                
            db.DeleteOwner(deleteOwner);
                
                
            return deleteOwner;
        });

    }
}
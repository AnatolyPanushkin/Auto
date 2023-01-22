// See https://aka.ms/new-console-template for more information

using AutoMessages;
using EasyNetQ;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace Auto.AuditLog
{
    class Program
    {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        private const string SUBSCRIBER_ID = "Auto.AuditLog";

        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<NewOwnerConsumer>(typeof(NewOwnerConsumerDefinition));

                        x.SetKebabCaseEndpointNameFormatter();

                        x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
                    });
                }).Build()
                .RunAsync();
            
            /*using var bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
            Console.WriteLine("Connected! Listening for NewVehicleMessage messages.");
            await bus.PubSub.SubscribeAsync<NewOwnerMessage>(SUBSCRIBER_ID, HandleNewOwnerMessage);*/
            Console.ReadKey(true);
        }

        private static void HandleNewOwnerMessage(NewOwnerMessage message)
        {
            var csv =
                $"{message.Name},{message.Surname},{message.PhoneNumber},{message.Email},{message.VehicleOfOwner},{message.ListedAtUtc:O}";
            Console.WriteLine(csv);
        }

        private static IConfigurationRoot ReadConfiguration()
        {
            var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
// See https://aka.ms/new-console-template for more information

using Auto.Data.Entities;
using AutoMessages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace Auto.NewOwnerNotifier
{
    class Program
    {
        private const string SIGNALR_HUB_URL = "http://localhost:5000/hub";
        private static HubConnection hub;
        
        private static readonly IConfigurationRoot config = ReadConfiguration();

        private const string SUBSCRIBER_ID = "Auto.NewOwnerNotifier";

        static async Task Main(string[] args)
        {
            hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
            await hub.StartAsync();
            
            using var bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
            Console.WriteLine("Connected! Listening for NewOwnerMessage messages.");
            
            await bus.PubSub.SubscribeAsync<NewOwnerMessageWithVehicle>(SUBSCRIBER_ID, HandleNewOwnerMessage);
            Console.ReadKey(true);
        }

        private static async Task HandleNewOwnerMessage(NewOwnerMessageWithVehicle message)
        {
            Console.WriteLine("Hub started!");
            Console.WriteLine("Press any key to send a message (Ctrl-C to quit)");
            var notifyMessage = JsonConvert.SerializeObject(new NewOwnerMessageWithVehicle()
            {
                    Email = message.Email,
                    Name = message.Name,
                    Surname = message.Surname,
                    VehicleModel = message.VehicleModel
            });
            await hub.SendAsync("NotifyWebUsersOwner", "Auto.Notifier", notifyMessage);
            Console.WriteLine($"Sent: {notifyMessage}");
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

using Auto.InfoOwnerServer;
using AutoMessages;
using EasyNetQ;
using Grpc.Net.Client;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Auto.InfoOwnerClient
{
    public static class Program
    {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        private const string SUBSCRIBER_ID = "Auto.InfoOwnerClient";

        private static IBus _bus;
            
      
        static async Task Main(String[] args)
        {
            _bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
            await _bus.PubSub.SubscribeAsync<NewOwnerMessage>(SUBSCRIBER_ID, HandleNewOwnerMessage);
            Console.ReadKey(true);
        }
        
        private static async Task HandleNewOwnerMessage(NewOwnerMessage message)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5082");
            
            var grpcClient = new OwnerInfo.OwnerInfoClient(channel);
            
            var request = new OwnerInfoRequest
            {
                Email = message.Email,
                Name = message.Name,
                Surname = message.Surname,
                Registration = message.VehicleOfOwner
            };

            var reply = grpcClient.GetOwnerInfo(request);

            /*var newOwnerMessageWithVehicle = new NewOwnerMessageWithVehicle
            {
                Email = reply.Email,
                Name = reply.Name,
                Surname = reply.Surname,
                VehicleModel = reply.Vehicle
            };*/
            await _bus.PubSub.PublishAsync(new NewOwnerMessageWithVehicle()
            {
                Email = reply.Email,
                Name = reply.Name,
                Surname = reply.Surname,
                VehicleModel = reply.Vehicle
            });
            //await Bus.PubSub.PublishAsync(newOwnerMessageWithVehicle);
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

﻿// See https://aka.ms/new-console-template for more information

using Auto.PricingService;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("http://localhost:5020");
var grpcClient = new Pricer.PricerClient(channel);
Console.WriteLine("Ready! Press any key to send a gRPCrequest (or Ctrl-C to quit):");
while (true)
{
    Console.ReadKey
        (true);
    var request = new PriceRequest
    {
        Model = "volkwsagen-beetle",
        Color = "Green",
        Year = 1985
    };
    var reply = grpcClient.GetPrice(request);
    Console.WriteLine($"Price: {reply.Price}");
}

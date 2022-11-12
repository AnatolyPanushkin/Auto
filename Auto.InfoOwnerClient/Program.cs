using Auto.InfoOwnerServer;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("http://localhost:5082");
var grpcClient = new OwnerInfo.OwnerInfoClient(channel);
Console.WriteLine("Ready! Press any key to send a gRPCrequest (or Ctrl-C to quit):");

Console.WriteLine("Enter email:");
var email = Console.ReadLine();
var request = new OwnerInfoRequest
{
    Email = email
};

var reply = grpcClient.GetOwnerInfo(request);
Console.WriteLine($"Owner info: {reply.Name} {reply.Surname} Vehicle registration number:{reply.Registration}");

using AutoMessages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Auto.AuditLog;

public class NewOwnerConsumer:IConsumer<NewOwnerMessage>
{
    readonly ILogger<NewOwnerConsumer> _logger;

    public NewOwnerConsumer(ILogger<NewOwnerConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewOwnerMessage> context)
    {
        _logger.LogInformation("Owner Email: {OrderId}", context.Message.Email);
        
        Console.WriteLine("Owner Email: {0}", context.Message.Email);
    }
}
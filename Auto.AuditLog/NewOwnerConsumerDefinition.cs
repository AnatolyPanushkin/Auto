using MassTransit;

namespace Auto.AuditLog;

public class NewOwnerConsumerDefinition : ConsumerDefinition<NewOwnerConsumer>
{
    public NewOwnerConsumerDefinition()
    {
        // override the default endpoint name
        EndpointName = "order-service";
    }

}
using Auto.PricingService;
using Grpc.Core;

namespace Auto.PricingServer.Services;

public class VehiclePricingService : Pricer.PricerBase
{
    private readonly ILogger<VehiclePricingService> _logger;

    public VehiclePricingService(ILogger<VehiclePricingService> logger)
    {
        _logger = logger;
    }

    public override Task<PriceReply> GetPrice(PriceRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PriceReply() {CurrencyCode = "RUB", Price = 400});
    }
    
}
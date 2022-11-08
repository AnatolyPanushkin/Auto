using Auto.InfoOwnerServer;
using Grpc.Core;
using Auto.Data;

namespace Auto.InfoOwnerServer.Services;

public class OwnerInfoService:OwnerInfo.OwnerInfoBase
{
    private readonly ILogger<OwnerInfoService> _logger;
    private readonly IAutoDatabase _db;

    public OwnerInfoService(ILogger<OwnerInfoService> logger, IAutoDatabase db)
    {
        _logger = logger;
        _db = db;
    }

    public override Task<OwnerInfoReply> GetOwnerInfo(OwnerInfoRequest request, ServerCallContext context)
    {
        var owner = _db.FindOwnerByEmail(request.Email);
        return Task.FromResult(new OwnerInfoReply() {Name = owner.Name, Surname = owner.Surname, Registration = owner.VehicleOfOwner.Registration});
    }
}
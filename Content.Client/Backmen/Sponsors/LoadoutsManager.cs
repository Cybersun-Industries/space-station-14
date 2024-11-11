using Content.Corvax.Interfaces.Shared;
using Robust.Shared.Network;

namespace Content.Client.Backmen.Sponsors;

public sealed class LoadoutsManager : ISharedLoadoutsManager
{
    [Dependency] private readonly ISharedSponsorsManager _sponsorsManager = default!;

    public void Initialize()
    {
    }

    public bool TryGetServerPrototypes(NetUserId userId, out List<string> prototypes)
    {
        throw new NotImplementedException();
    }

    public List<string> GetClientPrototypes()
    {
        return _sponsorsManager.GetClientLoadouts();
    }
}

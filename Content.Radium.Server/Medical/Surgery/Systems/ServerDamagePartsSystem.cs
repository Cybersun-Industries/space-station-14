using Content.Radium.Shared.Medical.Surgery.Systems;
using Content.Server.Body.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;

namespace Content.Radium.Server.Medical.Surgery.Systems;

public sealed class ServerDamagePartsSystem : DamagePartsSystem
{
    [Dependency] private readonly BodySystem _bodySystem = null!;
    public override IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildren(EntityUid euid, BodyComponent bodyComponent)
    {
        return _bodySystem.GetBodyChildren(euid, bodyComponent);
    }
}

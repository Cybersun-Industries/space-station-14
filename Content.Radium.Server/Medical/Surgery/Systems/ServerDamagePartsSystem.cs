using System.Linq;
using Content.Radium.Common.Medical.Surgery;
using Content.Server.Body.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.Nutrition.EntitySystems;

namespace Content.Radium.Server.Medical.Surgery.Systems;

public sealed class ServerDamagePartsSystem : EntitySystem, IServerDamagePartsSystem
{
    [Dependency] private readonly BodySystem _bodySystem = null!;

    public IReadOnlyDictionary<(Enum, Enum), (int, bool)> GetDamagedParts(EntityUid euid)
    {
        Dictionary<(Enum, Enum), (int, bool)> partsWounds = new(); //BodyPartType, BodyPartSymmetry
        if (!TryComp<BodyComponent>(euid, out var bodyComponent))
            return partsWounds;

        foreach (var (_, component) in GetBodyChildren<BodyPartComponent>(euid, bodyComponent))
        {
            var bodyPart = component;
            try
            {
                partsWounds.TryAdd((bodyPart.PartType, bodyPart.Symmetry),
                    (bodyPart.Wounds.Count, false)); //isDamaged
            }
            catch (Exception)
            {
                Logger.GetSawmill("Surgery").Error("Exception in adding parts!");
            }
        }

        return partsWounds;
    }

    public IEnumerable<(EntityUid Id, T bodyComponent)> GetBodyChildren<T>(EntityUid euid, Component bodyComponent) where T : IComponent
    {
        var result = new List<(EntityUid, T)>();
        var parts = _bodySystem.GetBodyChildren(euid, bodyComponent as BodyComponent).ToArray();
        foreach (var part in parts)
        {
            if (!TryComp<T>(part.Id, out var component))
                continue;

            result.Add((part.Id, component));
        }
        return result;
    }
}

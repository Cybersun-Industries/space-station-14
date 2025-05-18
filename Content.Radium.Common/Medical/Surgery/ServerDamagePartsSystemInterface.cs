using System.Collections;

namespace Content.Radium.Common.Medical.Surgery;

public interface IServerDamagePartsSystem
{
    IReadOnlyDictionary<(Enum, Enum), (int, bool)> GetDamagedParts(EntityUid euid); //BodyPartType, BodyPartSymmetry
    IEnumerable<(EntityUid Id, T bodyComponent)> GetBodyChildren<T>(EntityUid euid, Component bodyComponent) where T : IComponent;
}


namespace Content.Radium.Common.Medical.Surgery.Interfaces;

public interface IClientDamagePartsSystem
{
    IEnumerable<(EntityUid Id, T0 Component)> GetBodyChildren<T0, T1>(EntityUid euid,
        T1 bodyComponent) where T0 : IComponent;

    IReadOnlyDictionary<(T0, T1), (int, bool)> GetDamagedParts<T0, T1>(EntityUid euid)
        where T0 : struct where T1 : struct; //BodyPartType, BodyPartSymmetry
}

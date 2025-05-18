namespace Content.Radium.Common.Medical.Surgery.Interfaces;

public interface IServerDamagePartsSystem
{
    IReadOnlyDictionary<(Enum, Enum), (int, bool)> GetDamagedParts(EntityUid euid); //BodyPartType, BodyPartSymmetry
}


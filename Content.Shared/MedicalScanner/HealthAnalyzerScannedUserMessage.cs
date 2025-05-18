using Content.Radium.Common.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Shared.Serialization;

namespace Content.Shared.MedicalScanner;

/// <summary>
///     On interacting with an entity retrieves the entity UID for use with getting the current damage of the mob.
/// </summary>
[Serializable, NetSerializable]
public sealed class HealthAnalyzerScannedUserMessage : BoundUserInterfaceMessage
{
    public readonly NetEntity? TargetEntity;
    public float Temperature;
    public float BloodLevel;
    public bool? ScanMode;
    public bool? Bleeding;
    public bool? Unreviveable;
    public SurgeryStepData? SurgeryData;
    public IReadOnlyDictionary<(BodyPartType, BodyPartSymmetry), (int, bool)>? DamagedBodyParts;

    public HealthAnalyzerScannedUserMessage(NetEntity? targetEntity, float temperature, float bloodLevel, bool? scanMode, bool? bleeding, SurgeryStepData? surgeryStep, IReadOnlyDictionary<(BodyPartType, BodyPartSymmetry), (int, bool)>? damagedBodyParts, bool? unreviveable)
    {
        TargetEntity = targetEntity;
        Temperature = temperature;
        BloodLevel = bloodLevel;
        ScanMode = scanMode;
        Bleeding = bleeding;
        SurgeryData = surgeryStep;
        DamagedBodyParts = damagedBodyParts;
        Unreviveable = unreviveable;
    }
}

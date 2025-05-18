using Robust.Shared.Serialization;

namespace Content.Radium.Common.Medical.Surgery;

[RegisterComponent]
public sealed partial class SurgeryStepComponent : Component
{
    [DataField]
    public bool Repeatable;

    [DataField]
    public int RepeatIndex;

    [DataField]
    private LocId Name { get; set; }

    [DataField("desc", required: true)]
    private LocId Description { get; set; }

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedDescription => Loc.GetString(Description);

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedName => Loc.GetString(Name);

    [DataField]
    public string? Icon { get; set; }

    [DataField("action", required: true)]
    public Enum Key = null!;

    public int StepIndex;
}

[Serializable, NetSerializable]
public record struct SurgeryStepData
{
    public SurgeryStepData(SurgeryStepComponent? currentStep, string? operationName)
    {
        if (currentStep == null)
            return;
        LocalizedName = currentStep.LocalizedName;
        LocalizedDescription = currentStep.LocalizedDescription;
        Key = currentStep.Key;
        Icon = currentStep.Icon;
        if (operationName != null)
            OperationName = operationName;
    }

    public string OperationName = "";
    public string LocalizedName = "";
    public string LocalizedDescription = "";
    public string? Icon = null;
    public Enum Key = SurgeryTypeEnum.AddPart;
}

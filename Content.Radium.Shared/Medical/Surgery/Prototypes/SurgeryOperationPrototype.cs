using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Radium.Shared.Medical.Surgery.Prototypes;

[Prototype("surgeryOperation")]
public sealed class SurgeryOperationPrototype : IPrototype
{
    [IdDataField]
    [ViewVariables]
    public string ID { get; private set; } = null!;

    [DataField(required: true)]
    public LocId Name { get; set; }

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedName => Loc.GetString(Name);

    [DataField("desc", required: true)]
    public LocId Description { get; set; }

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedDescription => Loc.GetString(Description);

    [DataField("bodyPart")]
    public string BodyPart { get; set; } = "Head";

    [DataField]
    public SpriteSpecifier? Icon { get; private set; }

    [DataField("steps")]
    public List<Common.Medical.Surgery.SurgeryStepComponent>? Steps;

    [DataField("key")]
    public string EventKey = string.Empty;

    [DataField("unique")]
    public bool Unique;

    [DataField("hidden")]
    public bool IsHidden;
}

using Content.Radium.Shared.Medical.Surgery.Components;
using Content.Shared.Body.Part;

namespace Content.Radium.Server.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryInProgressComponent : Component
{
    [DataField]
    public SurgeryStepComponent? CurrentStep { get; set; }

    [DataField(readOnly: true)]
    public string? SurgeryPrototypeId { get; set; }

    public Enum Symmetry { get; set; } = BodyPartSymmetry.None;
}

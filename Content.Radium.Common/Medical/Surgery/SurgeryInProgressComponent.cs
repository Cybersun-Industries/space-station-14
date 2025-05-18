namespace Content.Radium.Common.Medical.Surgery;

[RegisterComponent]
public sealed partial class SurgeryInProgressComponent : Component
{
    [DataField]
    public SurgeryStepComponent? CurrentStep { get; set; }

    [DataField(readOnly: true)]
    public string? SurgeryPrototypeId { get; set; }

    public Enum Symmetry { get; set; }
}

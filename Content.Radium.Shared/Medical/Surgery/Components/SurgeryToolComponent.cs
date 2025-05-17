namespace Content.Radium.Shared.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryToolComponent: Component
{
    [DataField("action", required: true)]
    public Enum Key = null!;

    [DataField("modifier")]
    public float Modifier = 1f;
}

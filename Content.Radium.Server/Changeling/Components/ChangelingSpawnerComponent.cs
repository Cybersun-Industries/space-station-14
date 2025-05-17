namespace Content.Radium.Server.Changeling.Components;

[RegisterComponent]
public sealed partial class ChangelingSpawnerComponent: Component
{
    [DataField("target")]
    public EntityUid TargetForce { get; set; } = EntityUid.Invalid;
}

namespace Content.Radium.Server.Changeling.Components;

[RegisterComponent]
public sealed partial class ChangelingDnaComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public bool IsExtracted = false;
}

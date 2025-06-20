using Robust.Shared.GameStates;

namespace Content.Radium.Shared.VoidWalker;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class VoidShifterComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public bool InUse = false;

}

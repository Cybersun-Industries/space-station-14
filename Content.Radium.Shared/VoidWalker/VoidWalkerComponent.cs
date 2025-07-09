using Content.Shared.Physics;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;

namespace Content.Radium.Shared.VoidWalker;

[RegisterComponent]
[NetworkedComponent]

public partial class VoidWalkerComponent : Component
{

    [DataField]
    public bool IsActive;

}

[Serializable, NetSerializable]
public sealed class VoidWalkerComponentState : IComponentState
{
    public bool IsActive;
}

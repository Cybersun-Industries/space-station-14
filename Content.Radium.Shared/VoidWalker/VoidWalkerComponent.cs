using Content.Shared.Physics;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;

namespace Content.Radium.Shared.VoidWalker;

[RegisterComponent]
[NetworkedComponent]

public partial class VoidWalkerComponent : Component
{
    // For future development of seamless void shifting
    [DataField]
    public bool IsActive;

    [DataField]
    public int Collision = (int) CollisionGroup.Impassable;

}

[Serializable, NetSerializable]
public sealed class VoidWalkerComponentState : IComponentState
{
    public bool IsActive;
}

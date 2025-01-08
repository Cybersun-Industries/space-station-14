using Content.Shared.Actions;
using Content.Shared.Radium.Megafauna.Actions;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
namespace Content.Shared.Radium.Megafauna.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(AshDrakeGunSystem))]
public sealed partial class AshDrakeGunComponent : Component
{
    /// <summary>
    /// Action to create, must use <see cref="ActionGunShootEvent"/>.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId Action = string.Empty;

    [DataField]
    public EntityUid? ActionEntity;

    /// <summary>
    /// Prototype of gun entity to spawn.
    /// Deleted when this component is removed.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId GunProto = string.Empty;

    [DataField]
    public EntityUid? Gun;
}

/// <summary>
/// Action event for <see cref="ActionGunComponent"/> to shoot at a position.
/// </summary>
public sealed partial class AshDrakeGunShootEvent : WorldTargetActionEvent;

using Content.Shared.Actions;
using Content.Shared.Radium.Megafauna.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using ActionGunShootEvent = Content.Shared.Weapons.Ranged.Components.ActionGunShootEvent;

namespace Content.Shared.Radium.Megafauna.Actions;

/// <summary>
/// This handles...
/// </summary>
public sealed class AshDrakeGunSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AshDrakeGunComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<AshDrakeGunComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<AshDrakeGunComponent, AshDrakeGunShootEvent>(OnShoot);
    }

    private void OnMapInit(Entity<AshDrakeGunComponent> ent, ref MapInitEvent args)
    {
        if (string.IsNullOrEmpty(ent.Comp.Action))
            return;

        _actions.AddAction(ent, ref ent.Comp.ActionEntity, ent.Comp.Action);
        ent.Comp.Gun = Spawn(ent.Comp.GunProto);
    }

    private void OnShutdown(Entity<AshDrakeGunComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.Gun is {} gun)
            QueueDel(gun);
    }

    private void OnShoot(Entity<AshDrakeGunComponent> ent, ref AshDrakeGunShootEvent args)
    {
        if (TryComp<GunComponent>(ent.Comp.Gun, out var gun))
            _gun.AttemptShoot(ent, ent.Comp.Gun.Value, gun, args.Target);
    }
}

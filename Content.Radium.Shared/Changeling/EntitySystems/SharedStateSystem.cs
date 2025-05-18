using Content.Shared.Movement.Systems;
namespace Content.Radium.Shared.Changeling.EntitySystems;

public abstract class SharedGenstealerStateSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movement = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<Components.StateComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<Components.StateComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<Components.StateComponent, RefreshMovementSpeedModifiersEvent>(OnRefresh);
    }

    private void OnRefresh(EntityUid uid, Components.StateComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        args.ModifySpeed(component.MovementSpeedDebuff, component.MovementSpeedDebuff);
    }

    public virtual void OnStartup(EntityUid uid, Components.StateComponent component, ComponentStartup args)
    {
        _appearance.SetData(uid, ChangelingVisuals.Idle, true);
        _movement.RefreshMovementSpeedModifiers(uid);
    }

    public virtual void OnShutdown(EntityUid uid, Components.StateComponent component, ComponentShutdown args)
    {
        _appearance.SetData(uid, ChangelingVisuals.Idle, false);

        component.MovementSpeedDebuff = 1; //just so we can avoid annoying code elsewhere
        _movement.RefreshMovementSpeedModifiers(uid);
    }
}

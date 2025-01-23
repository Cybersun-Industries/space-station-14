using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates;
using Content.Shared.Doors.Components;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Power.Components;
using Content.Shared.PowerCell.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.StationAi;
using Content.Shared.UserInterface;
using Content.Shared.Wires;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Timing;

namespace Content.Shared.Silicons.Borgs;

/// <summary>
/// This handles logic, interactions, and UI related to <see cref="BorgChassisComponent"/> and other related components.
/// </summary>
public abstract partial class SharedBorgSystem : EntitySystem
{
    [Dependency] protected readonly SharedContainerSystem Container = default!;
    [Dependency] protected readonly ItemSlotsSystem ItemSlots = default!;
    [Dependency] protected readonly ItemToggleSystem Toggle = default!;
    [Dependency] protected readonly SharedPopupSystem Popup = default!;
    [Dependency] private readonly SharedContainerSystem _containers = default!;
    [Dependency] protected readonly SharedMapSystem Maps = default!;
    [Dependency] private readonly StationAiVisionSystem _vision = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly SharedPowerReceiverSystem _power = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedStationAiSystem _sharedStationAi = default!;

    private EntityQuery<BroadphaseComponent> _broadphaseQuery;
    private EntityQuery<MapGridComponent> _gridQuery;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BorgChassisComponent, AccessibleOverrideEvent>(OnBorgAccessible);
        SubscribeLocalEvent<BorgChassisComponent, InRangeOverrideEvent>(OnBorgInRange);

        SubscribeLocalEvent<BorgChassisComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<BorgChassisComponent, ItemSlotInsertAttemptEvent>(OnItemSlotInsertAttempt);
        SubscribeLocalEvent<BorgChassisComponent, ItemSlotEjectAttemptEvent>(OnItemSlotEjectAttempt);
        SubscribeLocalEvent<StationAiWhitelistComponent, InteractUsingEvent>(OnBorgInteraction);
        SubscribeLocalEvent<BorgChassisComponent, EntInsertedIntoContainerMessage>(OnInserted);
        SubscribeLocalEvent<BorgChassisComponent, EntRemovedFromContainerMessage>(OnRemoved);
        SubscribeLocalEvent<BorgChassisComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiers);
        SubscribeLocalEvent<BorgChassisComponent, ActivatableUIOpenAttemptEvent>(OnUIOpenAttempt);
        SubscribeLocalEvent<TryGetIdentityShortInfoEvent>(OnTryGetIdentityShortInfo);

        InitializeRelay();
    }


    // I wrote it myself. If you have found any bugs here, mail me. I wont respond or make any changes, but you could try to make me do so.
    private void OnBorgInteraction(Entity<StationAiWhitelistComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Target == ent.Owner)
            return;

        /*
        if (_interaction.TryGetUsedEntity(ent.Owner, out var used) == true)
        {
            if (TryComp<ToolComponent>(args.Used, out var _))
                _interaction.InteractHand(ent.Owner, args.Target);
        }
        */
        // _interaction.InteractHand(ent.Owner, args.);
        args.Handled = true;
        return;
    }

    private void OnBorgInRange(Entity<BorgChassisComponent> ent, ref InRangeOverrideEvent args)
    {
        if (ent == null || args.Target == null)
            return;

        var range = 1f;
        args.Handled = true;

        if (_interaction.InRangeUnobstructed(ent, args.Target.ToCoordinates(), range))
            args.InRange = true;

        if (!_power.IsPowered(args.Target))
            return;

        if (!HasComp<StationAiWhitelistComponent>(args.Target))
            return;

        args.InRange = true; // TODO: StationAI can alt-interact with doors. make the same thing you nerdo
    }

    // same as before. i dont know what this does. copy-pasta code.
    private void OnBorgAccessible(Entity<BorgChassisComponent> ent, ref AccessibleOverrideEvent args)
    {
        args.Handled = true;

        // Hopefully AI never needs storage
        if (_containers.TryGetContainingContainer(args.Target, out var targetContainer))
        {
            return;
        }

        if (!_containers.IsInSameOrTransparentContainer(args.User, args.Target, otherContainer: targetContainer))
        {
            return;
        }

        args.Accessible = true;
    }

    private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent args)
    {
        if (args.Handled)
        {
            return;
        }

        if (!HasComp<BorgChassisComponent>(args.ForActor))
        {
            return;
        }

        args.Title = Name(args.ForActor).Trim();
        args.Handled = true;
    }

    private void OnItemSlotInsertAttempt(EntityUid uid, BorgChassisComponent component, ref ItemSlotInsertAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<PowerCellSlotComponent>(uid, out var cellSlotComp) ||
            !TryComp<WiresPanelComponent>(uid, out var panel))
            return;

        if (!ItemSlots.TryGetSlot(uid, cellSlotComp.CellSlotId, out var cellSlot) || cellSlot != args.Slot)
            return;

        if (!panel.Open || args.User == uid)
            args.Cancelled = true;
    }

    private void OnItemSlotEjectAttempt(EntityUid uid, BorgChassisComponent component, ref ItemSlotEjectAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<PowerCellSlotComponent>(uid, out var cellSlotComp) ||
            !TryComp<WiresPanelComponent>(uid, out var panel))
            return;

        if (!ItemSlots.TryGetSlot(uid, cellSlotComp.CellSlotId, out var cellSlot) || cellSlot != args.Slot)
            return;

        if (!panel.Open || args.User == uid)
            args.Cancelled = true;
    }

    private void OnStartup(EntityUid uid, BorgChassisComponent component, ComponentStartup args)
    {
        if (!TryComp<ContainerManagerComponent>(uid, out var containerManager))
            return;

        component.BrainContainer = Container.EnsureContainer<ContainerSlot>(uid, component.BrainContainerId, containerManager);
        component.ModuleContainer = Container.EnsureContainer<Container>(uid, component.ModuleContainerId, containerManager);
    }

    private void OnUIOpenAttempt(EntityUid uid, BorgChassisComponent component, ActivatableUIOpenAttemptEvent args)
    {
        // borgs can't view their own ui
        if (args.User == uid)
            args.Cancel();
    }

    protected virtual void OnInserted(EntityUid uid, BorgChassisComponent component, EntInsertedIntoContainerMessage args)
    {

    }

    protected virtual void OnRemoved(EntityUid uid, BorgChassisComponent component, EntRemovedFromContainerMessage args)
    {

    }

    private void OnRefreshMovementSpeedModifiers(EntityUid uid, BorgChassisComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        if (Toggle.IsActivated(uid))
            return;

        if (!TryComp<MovementSpeedModifierComponent>(uid, out var movement))
            return;

        var sprintDif = movement.BaseWalkSpeed / movement.BaseSprintSpeed;
        args.ModifySpeed(1f, sprintDif);
    }

    /// <summary>
    /// Sets <see cref="BorgModuleComponent.DefaultModule"/>.
    /// </summary>
    public void SetBorgModuleDefault(Entity<BorgModuleComponent> ent, bool newDefault)
    {
        ent.Comp.DefaultModule = newDefault;
        Dirty(ent);
    }
}

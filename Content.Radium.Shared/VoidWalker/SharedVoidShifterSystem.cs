using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Timing;
using Robust.Shared.Map;

namespace Content.Radium.Shared.VoidWalker;

public class SharedVoidShifterSystem : EntitySystem
{
    [Dependency] private UseDelaySystem _delay = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoidShifterComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<VoidWalkerComponent, InRangeOverrideEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, BeforeRangedInteractEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, AccessibleOverrideEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, InteractEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, BeforeInteractHandEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidShifterComponent, GettingPickedUpAttemptEvent>(OnPickup);
        SubscribeLocalEvent<VoidShifterComponent, DroppedEvent>(OnDrop);
    }

    private bool InUse = true;

    private void OnPickup(EntityUid uid, VoidShifterComponent comp, GettingPickedUpAttemptEvent args)
    {
        if (!TryComp<MindContainerComponent>(args.User, out var mind) || !mind.HasMind || HasComp<VoidWalkerComponent>(args.User))
            return;
        var walkerComp = EnsureComp<VoidWalkerComponent>(args.User);
        walkerComp.IsActive = false;
    }

    private void OnDrop(EntityUid uid, VoidShifterComponent comp, DroppedEvent args)
    {
        RemComp<VoidWalkerComponent>(args.User);
    }

    private void OnUseInHand(EntityUid uid, VoidShifterComponent component, UseInHandEvent args)
    {

        if (!TryComp(uid, out UseDelayComponent? useDelay) || _delay.IsDelayed((uid, useDelay)))
            return; // shit doesnt work so i wont care anymore. TODO: finish this shit up

        if (!TryComp<VoidWalkerComponent>(args.User, out var walker))
            return;

        var ev = new VoidShiftingEvent(args.User, null);

        if (!component.InUse)
        {
            ev.Shifted = false;
            walker.IsActive = true;
            RaiseLocalEvent(uid, ev);

            component.InUse = true;

        }
        else
        {
            ev.Shifted = true;
            walker.IsActive = false;
            RaiseLocalEvent(uid, ev);

            component.InUse = false;

        }
    }


    /*
    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref AccessibleOverrideEvent args)
    {
        args.Handled = true;
    }
    */


    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref BeforeRangedInteractEvent args)
    {
        if (comp.IsActive)
            args.Handled = true;
        else
            args.Handled = false;
    }

    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref InRangeOverrideEvent args)
    {
        if (comp.IsActive)
        {
            args.Handled = true;
            args.InRange = false;
        }
        else
            args.Handled = false;
    }
    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref AccessibleOverrideEvent args)
    {
        if (comp.IsActive)
        {
            args.Handled = true;
            args.Accessible = false;
        }
        else
            args.Handled = false;
    }

    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref InteractEvent args)
    {
        if (comp.IsActive)
        {
            args.Handled = true;
        }
        else
            args.Handled = false;
    }

    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref BeforeInteractHandEvent args)
    {
        if (comp.IsActive)
        {
            args.Handled = true;
        }
        else
            args.Handled = false;
    }
}

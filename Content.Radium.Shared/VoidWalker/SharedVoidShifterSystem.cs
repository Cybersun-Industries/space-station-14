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

        // Okay some bullshittery up ahead that i NEED to fix. TODO: Xo6a, fix this.
        // We have two things that track whether a user is in void dimension:
        // A voidshifter component "in use" which indicates whether it was used to go to void dimension,
        // A voidwalker component "is active" which indicates, whether a user is IN the void dimension.
        // It is a quirky thing because we either check for voidshifter, or voidwalker components
        // to be active/in use, and it's also desynced (code quality moment, check code below).
        // I had a though that voidshifter should be "bound" to a user once it's used,
        // but this is a bullshit idea which came up with a mistake.

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

    // HandledEntityEventArgs all have the same parameter "Handled". I need to enable/disable all of them so...
    // Unfortunately I cant do the same with record structs because they use other custom-named vars :(
    private void InteractVoidWalker<T>(EntityUid uid, VoidWalkerComponent comp, ref T args) where T : HandledEntityEventArgs
    {
        if (comp.IsActive)
        {
            args.Handled = true;
        }
        else
            args.Handled = false;
    }
}


using Content.Shared.Hands;
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

public abstract partial class SharedVoidShifterSystem : EntitySystem // <-- void shitter
{
    [Dependency] private UseDelaySystem _delay = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeInteractionBlockers();

        SubscribeLocalEvent<VoidShifterComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<VoidShifterComponent, GotEquippedHandEvent>(OnEquip);
        SubscribeLocalEvent<VoidShifterComponent, GotUnequippedHandEvent>(OnUnequip); // BUG: this function raises when switching item from one hand to other
    }

    private void OnEquip(EntityUid uid, VoidShifterComponent comp, GotEquippedHandEvent args)
    {
        if (!TryComp<MindContainerComponent>(args.User, out var mind))
            return;

        if (!mind.HasMind)
            return;

        if (HasComp<VoidWalkerComponent>(args.User))
            return;

        var walkerComp = EnsureComp<VoidWalkerComponent>(args.User);
        walkerComp.IsActive = false;
    }

    private void OnUnequip(EntityUid uid, VoidShifterComponent comp, UnequippedHandEvent args)
    {
        if (!TryComp<VoidWalkerComponent>(args.User, out var walker))
        {
            Log.Error("[Voidwalker] An entity {0} does not have voidwalker component! It should have one because it's getting applied when you pick up voidshifter", args.User);
        }
        RemComp<VoidWalkerComponent>(args.User);
    }

    private void OnUseInHand(EntityUid uid, VoidShifterComponent component, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp(uid, out UseDelayComponent? useDelay) || _delay.IsDelayed((uid, useDelay)))
            return;

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
            // walker.IsActive = true;
            RaiseLocalEvent(uid, ev, true);

            component.InUse = true;
            component.User = args.User;

        }
        else
        {
            ev.Shifted = true;
            // walker.IsActive = false;
            RaiseLocalEvent(uid, ev, true);

            component.InUse = false;
            component.User = null;

        }
    }

}


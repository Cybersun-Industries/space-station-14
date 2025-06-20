using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Robust.Shared.Map;

namespace Content.Radium.Shared.VoidWalker;

public class SharedVoidShifterSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoidShifterComponent, UseInHandEvent>(OnUseInHand);
        // SubscribeLocalEvent<VoidWalkerComponent, AccessibleOverrideEvent>(InteractVoidWalker); // for future
    }

    private bool InUse = true;

    private void OnUseInHand(EntityUid uid, VoidShifterComponent component, UseInHandEvent args)
    {
        var ev = new VoidShiftingEvent(args.User, null);

        if (!component.InUse)
        {
            ev.Shifted = false;
            RaiseLocalEvent(uid, ev);

            component.InUse = true;
        }
        else
        {
            ev.Shifted = true;
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


}

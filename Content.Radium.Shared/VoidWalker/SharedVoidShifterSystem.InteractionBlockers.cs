using Content.Shared.Interaction;

namespace Content.Radium.Shared.VoidWalker;

public abstract partial class SharedVoidShifterSystem
{
    /// <inheritdoc/>
    public void InitializeInteractionBlockers()
    {
        SubscribeLocalEvent<VoidWalkerComponent, InRangeOverrideEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, BeforeRangedInteractEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, AccessibleOverrideEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, InteractEvent>(InteractVoidWalker);
        SubscribeLocalEvent<VoidWalkerComponent, BeforeInteractHandEvent>(InteractVoidWalker);
    }

    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref InRangeOverrideEvent args)
    {
        if (comp.IsActive)
        {
            args.Handled = true;
            args.InRange = false;
        }
    }
    private void InteractVoidWalker(EntityUid uid, VoidWalkerComponent comp, ref AccessibleOverrideEvent args)
    {
        if (comp.IsActive)
        {
            args.Handled = true;
            args.Accessible = false;
        }
    }

    // HandledEntityEventArgs all have the same parameter "Handled". I need to enable/disable all of them so...
    // Unfortunately I cant do the same with record structs because they use other custom-named vars :(
    private void InteractVoidWalker<T>(EntityUid uid, VoidWalkerComponent comp, ref T args) where T : HandledEntityEventArgs
    {
        if (comp.IsActive)
        {
            args.Handled = true;
        }
    }
}

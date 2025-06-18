using Content.Shared.Interaction.Events;

namespace Content.Radium.Shared.VoidWalker;

public sealed class SharedVoidShifterSystem : EntitySystem
{

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoidShifterComponent, UseInHandEvent>(OnUseInHand);
    }

    private void OnUseInHand(EntityUid uid, VoidShifterComponent? component, UseInHandEvent args)
    {
        if (HasComp<VoidWalkerComponent>(args.User))
        {
            RemComp<VoidWalkerComponent>(args.User);
            return;
        }
        EnsureComp<VoidWalkerComponent>(args.User);
    }
}

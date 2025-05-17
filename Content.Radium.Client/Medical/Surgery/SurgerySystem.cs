using Content.Radium.Shared.Medical.Surgery.Events;

namespace Content.Radium.Client.Medical.Surgery;

public sealed class SurgerySystem: EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BeginSurgeryEvent>(OnSurgeryBegin);

    }

    private void OnSurgeryBegin(BeginSurgeryEvent ev)
    {
        //Don't ask me about that
        RaiseNetworkEvent(ev);
    }
}


using Content.Radium.Client.IoC;

namespace Content.Radium.Client.Entry;

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

public sealed class EntryPoint : GameClient
{
    public override void Init()
    {
        ContentRadiumClientIoC.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
    }
}

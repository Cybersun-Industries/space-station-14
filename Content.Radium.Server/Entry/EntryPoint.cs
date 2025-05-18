using Content.Radium.Server.IoC;

namespace Content.Radium.Server.Entry;

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();

        ServerRadiumContentIoC.Register();

        IoCManager.BuildGraph();
    }
}

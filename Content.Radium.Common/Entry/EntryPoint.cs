using Content.Radium.Common.IoC;
using Robust.Shared.ContentPack;

namespace Content.Radium.Common.Entry;

// EntryPoint is marked as GameShared for module registration purposes.
public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        IoCManager.InjectDependencies(this);
        CommonRadiumContentIoC.Register();
    }
}

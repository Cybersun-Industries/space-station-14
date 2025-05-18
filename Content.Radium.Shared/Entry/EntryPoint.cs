using Content.Radium.Shared.IoC;
using Robust.Shared.ContentPack;

namespace Content.Radium.Shared.Entry;

// EntryPoint is marked as GameShared for module registration purposes.
public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        IoCManager.InjectDependencies(this);
        CommonRadiumContentIoC.Register();
    }
}

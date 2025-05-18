using Content.Radium.Common.Medical.Surgery;

namespace Content.Radium.Client.IoC;

using Robust.Shared.IoC;

internal static class ContentRadiumClientIoC
{
    internal static void Register()
    {
        var collection = IoCManager.Instance!;

        //collection.Register<IClientDamagePartsSystem, ClientDamagePartsSystem>();
    }
}

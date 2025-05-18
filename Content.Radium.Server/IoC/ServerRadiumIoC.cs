using Content.Radium.Common.Medical.Surgery;
using Content.Radium.Server.Medical.Surgery.Systems;
using Content.Server.Body.Systems;
using Content.Shared.Body.Systems;

namespace Content.Radium.Server.IoC;

internal static class ServerRadiumContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
        //instance.Register<IServerDamagePartsSystem, ServerDamagePartsSystem>();
        //instance.Register<SharedBodySystem, SharedBodySystem>();
    }
}

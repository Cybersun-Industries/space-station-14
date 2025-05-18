namespace Content.Radium.Shared.IoC;

using Robust.Shared.IoC;

internal static class CommonRadiumContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
    }
}

using Robust.Shared.Configuration;

namespace Content.Radium.Common.CCVar;

[CVarDefs]
public sealed partial class RadiumCVars
{
    public static readonly CVarDef<int> AutodebugEnergyMinPlayerThreshold =
        CVarDef.Create("autodebug.energy.minPlayerThreshold", 5, CVar.SERVER);
}

using Robust.Shared.Configuration;

namespace Content.Radium.Common.CCVar;

[CVarDefs]
public sealed partial class RadiumCVars
{
    public static readonly CVarDef<int> AutodebugEnergyMinPlayerThreshold =
        CVarDef.Create("autodebug.energy.minPlayerThreshold", 5, CVar.SERVER);

    public static readonly CVarDef<int> AutodebugAccessMinPlayerThreshold =
        CVarDef.Create("autodebug.access.minPlayerThreshold", 5, CVar.SERVER);

    public static readonly CVarDef<int> AutodebugVoteTime =
        CVarDef.Create("autodebug.time", 60, CVar.SERVER);

    public static readonly CVarDef<int> AutodebugVoteCooldown =
        CVarDef.Create("autodebug.cooldown", 1800, CVar.SERVER);
}

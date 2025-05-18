using Robust.Shared.Configuration;

namespace Content.Radium.Common.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<int> ViewportMaximumHeight =
        CVarDef.Create("viewport.maximum_height", 21, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> ViewportMinimumHeight =
        CVarDef.Create("viewport.minimum_height", 15, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> ViewportHeight =
        CVarDef.Create("viewport.height", 21, CVar.CLIENTONLY | CVar.ARCHIVE);
}

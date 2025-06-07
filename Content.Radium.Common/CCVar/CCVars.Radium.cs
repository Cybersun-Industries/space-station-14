// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

namespace Content.Radium.Common.CCVar;

[CVarDefs]
public sealed partial class RadiumCVars
{
    #region Autodebug

    /// <summary>
    /// Controls if autodebug vote should be called on job assign.
    /// May irritate during developing.
    /// </summary>
    public static readonly CVarDef<bool> AutodebugRoundstartVoteEnabled =
        CVarDef.Create("autodebug.vote.roundstart", true, CVar.SERVERONLY);

    /// <summary>
    /// Maximum players count for energy vote to appear.
    /// </summary>
    public static readonly CVarDef<int> AutodebugEnergyMaxPlayerThreshold =
        CVarDef.Create("autodebug.energy.maxPlayerThreshold", 5, CVar.SERVERONLY);

    /// <summary>
    /// Maximum players count for access vote to appear.
    /// </summary>
    public static readonly CVarDef<int> AutodebugAccessMaxPlayerThreshold =
        CVarDef.Create("autodebug.access.maxPlayerThreshold", 5, CVar.SERVERONLY);

    /// <summary>
    /// Duration (in seconds) the debug vote popup remains active.
    /// </summary>
    public static readonly CVarDef<int> AutodebugVoteTime =
        CVarDef.Create("autodebug.vote.time", 60, CVar.SERVERONLY);

    /// <summary>
    /// The cooldown period (in seconds) between vote or command executions.
    /// Applies globally across the entire server.
    /// </summary>
    public static readonly CVarDef<int> AutodebugVoteCooldown =
        CVarDef.Create("autodebug.cooldown", 1800, CVar.SERVERONLY);

    #endregion

    #region CustomizableStamps

    /// <summary>
    /// Max stamp text length
    /// </summary>
    public static readonly CVarDef<int> StampsMaxTextLength = CVarDef.Create("stamps.maxTextLength", 32, CVar.REPLICATED);

    #endregion
}

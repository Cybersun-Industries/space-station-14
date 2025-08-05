// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 Ilysha998 <sukhachew.ilya@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

namespace Content.Shared.Corvax.CCCVars;

/// <summary>
///     Corvax modules console variables
/// </summary>
[CVarDefs]
// ReSharper disable once InconsistentNaming
public sealed class CCCVars
{
/*
 * Station Goal
 */

/// <summary>
/// Send station goal on round start or not.
/// </summary>
public static readonly CVarDef<bool> StationGoal =
    CVarDef.Create("game.station_goal", true, CVar.SERVERONLY);
}

// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 Ilysha998 <sukhachew.ilya@gmail.com>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Corvax.StationGoal;
using Robust.Shared.Prototypes;

namespace Content.Server._Corvax.StationGoal;

/// <summary>
///     if attached to a station prototype, will send the station a random goal from the list
/// </summary>
[RegisterComponent]
public sealed partial class StationGoalComponent : Component
{
    [DataField]
    public List<ProtoId<StationGoalPrototype>> Goals = new();
}

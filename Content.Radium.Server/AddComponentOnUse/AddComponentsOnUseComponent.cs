// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 MorDast1 <morgit1488@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Server.ADT.AddComponentsOnUse;

[RegisterComponent]
public sealed partial class AddComponentsOnUseComponent : Component
{

    [DataField(required: true)]
    public ComponentRegistry Components = new();

    [DataField]
    public bool DeleteOnUse = true;
}

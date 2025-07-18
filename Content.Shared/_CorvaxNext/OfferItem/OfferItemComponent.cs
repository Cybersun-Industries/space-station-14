// SPDX-FileCopyrightText: 2024 AwareFoxy <135021509+AwareFoxy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._CorvaxNext.OfferItem;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedOfferItemSystem))]
public sealed partial class OfferItemComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public bool IsInOfferMode;

    [DataField, AutoNetworkedField]
    public bool IsInReceiveMode;

    [DataField, AutoNetworkedField]
    public string? Hand;

    [DataField, AutoNetworkedField]
    public EntityUid? Item;

    [DataField, AutoNetworkedField]
    public EntityUid? Target;

    [DataField]
    public float MaxOfferDistance = 2f;
}

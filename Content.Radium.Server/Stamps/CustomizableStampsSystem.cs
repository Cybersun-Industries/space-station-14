// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Common.CCVar;
using Content.Radium.Shared.Stamps;
using Content.Radium.Shared.Stamps.UI;
using Content.Shared.Examine;
using Content.Shared.Paper;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;

namespace Content.Radium.Server.Stamps;

public sealed class CustomizableStampsSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _configuration = null!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StampComponent, CustomizableStampMessage>(OnStampCustomization);
        SubscribeLocalEvent<CustomizableStampComponent, BeforeActivatableUIOpenEvent>(OnMenuOpen);

        SubscribeLocalEvent<CustomizableStampComponent, ExaminedEvent>(OnExamine);
    }

    private void OnExamine(EntityUid uid, CustomizableStampComponent component, ExaminedEvent args)
    {
        if (!TryComp<StampComponent>(uid, out var stampComponent))
            return;

        args.PushMarkup(
            $"\n{Loc.GetString("stamp-customization-examine", ("color", stampComponent.StampedColor.ToHexNoAlpha()), ("label", stampComponent.StampedName))}\n");
    }

    private void OnMenuOpen(EntityUid uid, CustomizableStampComponent component, BeforeActivatableUIOpenEvent args)
    {
        if (!TryComp<StampComponent>(uid, out var stampComponent))
            return;

        var newState = new CustomizableStampBoundUserInterfaceState(
            stampComponent.StampedColor,
            stampComponent.StampedName
        );

        _userInterface.SetUiState(uid, CustomizableStampUiKey.Key, newState);
    }

    private void OnStampCustomization(EntityUid uid, StampComponent component, CustomizableStampMessage args)
    {
        if (!HasComp<CustomizableStampComponent>(uid))
            return;

        var maxLength = _configuration.GetCVar(RadiumCVars.StampsMaxTextLength);

        if (args.Text.Length > maxLength)
            return;

        component.StampedColor = args.Color;
        component.StampedName = args.Text;
    }
}

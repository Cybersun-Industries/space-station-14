// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 pathetic meowmeow <uhhadd@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Cargo.UI;
using Content.Shared.Cargo.Components;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Cargo.BUI;

[UsedImplicitly]
public sealed class FundingAllocationConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private FundingAllocationMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<FundingAllocationMenu>();

        _menu.OnSavePressed += (dicts, primary, lockbox) =>
        {
            SendMessage(new SetFundingAllocationBuiMessage(dicts, primary, lockbox));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState message)
    {
        base.UpdateState(message);

        if (message is not FundingAllocationConsoleBuiState state)
            return;

        _menu?.Update(state);
    }
}

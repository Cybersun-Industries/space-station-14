// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Shared.Stamps.UI;
using Content.Shared.Paper;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Radium.Client.Stamps.UI;

[UsedImplicitly]
public sealed class CustomizableStampBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entityManager = null!;

    private StampCustomizationMenu? _menu;

    public CustomizableStampBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<StampCustomizationMenu>();
        _menu.OnApply += () =>
        {
            if(!_entityManager.TryGetComponent<StampComponent>(Owner, out var stampComponent))
                return;

            if (string.IsNullOrWhiteSpace(_menu.Text))
                return;

            stampComponent.StampedColor = _menu.Color;
            stampComponent.StampedName = _menu.Text;

            SendMessage(new CustomizableStampMessage(_menu.Color, _menu.Text));
            Close();
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        var castState = (CustomizableStampBoundUserInterfaceState) state;
        _menu?.UpdateState(castState);
    }
}

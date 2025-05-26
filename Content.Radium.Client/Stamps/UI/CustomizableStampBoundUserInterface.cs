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

using Content.Radium.Common.CCVar;
using Content.Radium.Common.Stamps;
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

        SubscribeLocalEvent<CustomizableStampComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<StampComponent, CustomizableStampMessage>(OnStampCustomization);
        SubscribeLocalEvent<CustomizableStampComponent, BeforeActivatableUIOpenEvent>(OnMenuOpen);

        SubscribeLocalEvent<CustomizableStampComponent, ExaminedEvent>(OnExamine);
    }

    private void OnComponentStartup(EntityUid uid, CustomizableStampComponent component, ComponentStartup args)
    {
        if (!TryComp<StampComponent>(uid, out var stampComponent))
            return;

        if (!Loc.TryGetString(stampComponent.StampedName, out var localizedName))
            return;

        stampComponent.StampedName = localizedName;
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

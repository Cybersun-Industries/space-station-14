using Content.Shared.Access.Systems;
using Content.Shared.Radium.Nanites;
using Content.Shared.Radium.Nanites.Components;
using Robust.Client.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client.Radium.Nanites.NanitesConsoles.Programmer;

public sealed class NProgrammerConsoleBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    private readonly AccessReaderSystem _accessReader;

    private NProgrammerConsoleWindow? _window;

    public NProgrammerConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _accessReader = EntMan.System<AccessReaderSystem>();
    }

    protected override void Open()
    {
        base.Open();

        var comp = EntMan.GetComponent<NProgrammerConsoleComponent>(Owner);

        _window = new(Owner, comp.MaxStringLength, _playerManager, _proto, _random, _accessReader);
        _window.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not NProgrammerConsoleState cast)
            return;

        // _window?.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _window?.Close();
    }
}

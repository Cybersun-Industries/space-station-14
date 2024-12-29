using Content.Server.Popups;
using Content.Server.Radio.EntitySystems;
using Content.Server.Station.Systems;
using Content.Shared.Access.Systems;
using Content.Shared.Radium.Nanites;
using Content.Shared.Radium.Nanites.Components;
using Content.Shared.Radium.Nanites.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.IdentityManagement;

namespace Content.Server.Radium.Nanites.Systems;

/// <summary>
/// Handles all UI for criminal records console
/// </summary>
public sealed class NCloudControllerConsoleSystem : SharedNanitesConsoleSystem
{
    [Dependency] private readonly AccessReaderSystem _access = default!;
    [Dependency] private readonly NanitesSystem _nanites = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly SharedIdCardSystem _idCard = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;


    public override void Initialize()
    {
        Subs.BuiEvents<NCloudControllerConsoleComponent>(NCloudControllerConsoleKey.Key,
            subs =>
            {
                subs.Event<BoundUIOpenedEvent>(UpdateUserInterface);

            });
    }

    private void UpdateUserInterface<T>(Entity<NCloudControllerConsoleComponent> ent, ref T args)
    {
        UpdateUserInterface(ent);
    }

    private void UpdateUserInterface(Entity<NCloudControllerConsoleComponent> ent)
    {
        var (uid, console) = ent;
        var owningStation = _station.GetOwningStation(uid);

        if (!TryComp<NCloudControllerConsoleComponent>(owningStation, out var stationRecords))
        {
            return;
        }

    }

}

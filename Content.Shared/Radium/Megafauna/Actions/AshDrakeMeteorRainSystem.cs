using Content.Shared.Actions.BaseActionEvent;
using Content.Shared.Directions;
using Content.Shared.Maps;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Radium.Megafauna.Components;
using Content.Shared.Stunnable;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Animations;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Radium.Megafauna.Actions;

/// <summary>
/// This handles making entities perform a meteor rain when the Ash Drake action is triggered.
/// </summary>
public sealed class AshDrakeMeteorRainSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;



    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<AshDrakeMeteorRainAction>(OnMeteorRainAction);
    }

    private void OnMeteorRainAction(AshDrakeMeteorRainAction args)

        {

            _popup.PopupPredicted(Loc.GetString("meteorrain-ability-use-popup", ("entity", args.Performer)),
                args.Performer,
                args.Performer,
                type: PopupType.SmallCaution);

            List<EntityCoordinates> spawnPos = new();
            spawnPos.Add(coords);

            var dirs = new List<Direction>();
            dirs.AddRange(args.OffsetDirections);

            for (var i = 0; i < 4; i++)
            {
                var dir = _random.PickAndTake(dirs);
                spawnPos.Add(coords.Offset(dir));
            }

            if (_transform.GetGrid(coords) is not { } grid || !TryComp<MapGridComponent>(grid, out var gridComp))
                return;

            foreach (var pos in spawnPos)
            {
                if (!_map.TryGetTileRef(grid, gridComp, pos, out var tileRef) ||
                    tileRef.IsSpace() ||
                    _turf.IsTileBlocked(tileRef, CollisionGroup.Impassable))
                {
                    continue;
                }

                if (_net.IsServer)
                    Spawn(args.EntityId, pos);
            }


        }
    }

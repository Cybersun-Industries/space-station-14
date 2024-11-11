using System.Linq;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Power.Components;
using Content.Server.Station.Systems;
using Content.Server.Backmen.StationEvents.Components;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Events;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.GameTicking.Components;
using Content.Shared.Physics;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Dynamics;

namespace Content.Server.Backmen.StationEvents.Events;

internal sealed class FreeProberRule : StationEventSystem<FreeProberRuleComponent>
{
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly AnchorableSystem _anchorable = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;

    private static readonly string ProberPrototype = "GlimmerProber";
    private static readonly int SpawnDirections = 4;

    protected override void Started(EntityUid uid, FreeProberRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        List<Entity<TransformComponent>> possibleSpawns = new();

        if (possibleSpawns.Count <= 0)
            return;

        _robustRandom.Shuffle(possibleSpawns);

        foreach (var source in possibleSpawns)
        {
            var xform = source.Comp;

            var coordinates = xform.Coordinates;
            if (!TryComp<MapGridComponent>(xform.GridUid, out var grid))
                continue;


            var mobTile = _mapSystem.GetTileRef(xform.GridUid.Value, grid, coordinates);

            for (var i = 0; i < SpawnDirections; i++)
            {
                var direction = (DirectionFlag) (1 << i);
                var offsetIndices = mobTile.GridIndices.Offset(direction.AsDir());
                var offsetMobTile = _mapSystem.GetTileRef(xform.GridUid.Value, grid, offsetIndices);

                if(offsetMobTile.Tile.IsEmpty)
                    continue;

                // This doesn't check against the prober's mask/layer, because it hasn't spawned yet...
                if (!_anchorable.TileFree(grid, offsetIndices, (int)CollisionGroup.WallLayer, (int)CollisionGroup.MachineMask))
                    continue;

                Spawn(ProberPrototype, _mapSystem.GridTileToLocal(xform.GridUid.Value, grid, offsetIndices));
                return;
            }
        }
    }
}

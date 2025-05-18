using System.Text;
using Content.Radium.Server.Changeling.Components;
using Content.Radium.Server.Changeling.EntitySystems;
using Content.Radium.Shared.Changeling.Components;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.StationEvents.Events;
using Content.Shared.GameTicking.Components;

namespace Content.Radium.Server.Changeling.StationEvents;

public sealed class ChangelingRule : StationEventSystem<ChangelingRuleComponent>
{
    [Dependency] private readonly ChangelingSystem _changelingSystem = null!;
    [Dependency] private readonly MindSystem _mindSystem = null!;
    [Dependency] private readonly ObjectivesSystem _objectivesSystem = null!;

    protected override void Started(EntityUid uid,
        ChangelingRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!_changelingSystem.MakeChangeling(out _))
        {
            Sawmill.Warning("Map not have latejoin pinpoints for creating changeling spawner");
        }
    }

    protected override void AppendRoundEndText(EntityUid uid,
        ChangelingRuleComponent changeling,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent ev)
    {
        var mostAbsorbedName = string.Empty;
        var mostStolenName = string.Empty;
        var mostAbsorbed = 0f;
        var mostStolen = 0f;

        ev.AddLine(Loc.GetString("changeling-prepend-title"));
        foreach (var ling in EntityQuery<ChangelingComponent>())
        {
            var mind = ling.Mind;

            if (mind == null)
                continue;

            var mindEntity = GetEntity(mind.OriginalOwnedEntity);

            if (!mindEntity.HasValue)
                continue;

            var hasMind = _mindSystem.TryGetMind(mindEntity.Value, out var mindId, out _);
            {
                if (ling.TotalAbsorbedEntities > mostAbsorbed)
                {
                    mostAbsorbed = ling.TotalAbsorbedEntities;
                    if (hasMind)
                        mostAbsorbedName = _objectivesSystem.GetTitle((mindId, mind), string.Empty);
                }

                if (!(ling.TotalExtractedDna > mostStolen))
                    continue;

                mostStolen = ling.TotalExtractedDna;

                if (hasMind)
                    mostStolenName = _objectivesSystem.GetTitle((mindId, mind), string.Empty);
            }
        }

        var sb = new StringBuilder();
        if (mostAbsorbed != 0)
        {
            sb.AppendLine(Loc.GetString(
                $"roundend-prepend-changeling-absorbed{(!string.IsNullOrWhiteSpace(mostAbsorbedName) ? "-named" : "")}",
                ("name", mostAbsorbedName),
                ("number", mostAbsorbed)));
        }

        if (mostStolen != 0)
        {
            sb.AppendLine(Loc.GetString(
                $"roundend-prepend-changeling-stolen{(!string.IsNullOrWhiteSpace(mostStolenName) ? "-named" : "")}",
                ("name", mostStolenName),
                ("number", mostStolen)));
        }

        ev.AddLine(sb.ToString());
    }
}

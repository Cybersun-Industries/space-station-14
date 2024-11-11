﻿using Content.Server.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Server.Radium.Changeling.Components;
using Content.Shared.Objectives.Components;

namespace Content.Server.Radium.Changeling.EntitySystems;

public sealed class ChangelingConditionsSystem : EntitySystem
{
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<GenesConditionComponent, ObjectiveGetProgressEvent>(OnGenesGetProgress);
    }

    private void OnGenesGetProgress(EntityUid uid, GenesConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = GenesProgress(comp, _number.GetTarget(uid));
    }

    private float GenesProgress(GenesConditionComponent comp, int target)
    {
        // prevent divide-by-zero
        return target == 0 ? 1f : MathF.Min(comp.GenesExtracted / (float) target, 1f);
    }
}

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Radium.Nanites;
using Content.Shared.Radium.Nanites.Components;
using Content.Shared.Radium.Nanites.Events;
using Content.Server.GameTicking;
using Content.Shared.Alert;

using System.Numerics;
using Content.Server.Preferences.Managers;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;

namespace Content.Server.Radium.Nanites.Systems;

public sealed class NanitesSystem : EntitySystem
{
    [Dependency] private readonly NanitesPowerSystem _power = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;




    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanitesComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<NanitesComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<NanitesComponent, ComponentShutdown>(OnShutdown);
    }


    private void OnExamine(EntityUid uid, NanitesComponent component, ExaminedEvent args)
    {
        // so if we examine ourselves we get a power level of nanites
        if (args.Examined == args.Examiner)
        {
            args.PushMarkup(Loc.GetString("nanites-powerlevel-examined",
                ("power", (int) component.NanitesLevel),
                ("powerMax", component.PowerLevelMax)
            ));
        }
    }

    private void OnInit(EntityUid uid, NanitesComponent component, ComponentInit args)
    {
        if (component.NanitesLevel <= NanitesComponent.PowerThresholds[NanitesThreshold.Min] + 1f)
            // ensure that we dont have 0 nanites at the start.
            // potentially a disaster (mass abuse)!!!
            _power.SetPowerLevel(uid, NanitesComponent.PowerThresholds[NanitesThreshold.Good]);

        _power.UpdateAlert(uid, true, component.NanitesLevel);
    }

    private void OnShutdown(EntityUid uid, NanitesComponent component, ComponentShutdown args)
    {
        _power.UpdateAlert(uid, false);
    }


    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<NanitesComponent>();

        // Update power level for all nanite users
        while (query.MoveNext(out var uid, out var nanites))
        {
            var oldPowerLevel = _power.GetLevelName(nanites.NanitesLevel);
            _power.TryUpdatePowerLevel(uid, frameTime);

            if (oldPowerLevel != _power.GetLevelName(nanites.NanitesLevel))
            {
                Dirty(uid, nanites);
            }
            _power.UpdateAlert(uid, true, nanites.NanitesLevel);
        }
    }
}

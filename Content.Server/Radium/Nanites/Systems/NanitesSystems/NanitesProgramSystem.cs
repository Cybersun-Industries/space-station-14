using Content.Server.Radium.Medical.Surgery.Systems;
using Content.Shared.Radium.Nanites.Systems;
using Content.Shared.Radium.Nanites.Events;
using Robust.Shared.Timing;
using Content.Shared.Alert;
using Content.Shared.Radium.Nanites.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Commands.Math;
using Robust.Shared.Utility;
using Content.Shared.Damage;
using Content.Server.Body.Systems;
using Content.Shared.Prototypes;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.Damage.Prototypes;
using FastAccessors;

namespace Content.Server.Radium.Nanites.Programs;

public abstract class NanitesServerPrograms : SharedNanitesSystem
{
    [Dependency] private readonly SharedNanitesSystem _nanitesSystem = default!;
    [Dependency] private readonly IGameTiming _gameTicker = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SolutionContainerSystem _solutions = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly DamageSpecifier _specifier = default!;
    [Dependency] private readonly NanitesComponent _component = default!;
    [Dependency] private readonly ISawmill _logger = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    private DamageSpecifier? _damageSpec;

    private void RegenerateHealthByNanites(EntityUid uid, EntityUid user, DamageSpecifier dmgTypes)
    {
            _damage.TryChangeDamage(uid, dmgTypes);
    }

    public bool TryRegenerateHealthByNanites(EntityUid uid, EntityUid user, NanitesComponent component, float damage, string[]? specifier)
    {
        var damageable = EnsureComp<DamageableComponent>(uid);
        if (uid == null || user == null || component == null || damageable == null)
        {
            _logger.Debug("NanitesSystem: not enough arguments");
            return false;
        }
        /*                                              thx chad-gpt B)
         if (specifier == null)
        {
            _damageSpec = new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Brute"), damage);
        }
        else if (specifier == default)
        {
            _damageSpec = new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Brute"), damage / 2);
            _damageSpec += new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Physical"), damage / 2);
        }
        else
        {
            _damageSpec = new DamageSpecifier();
            foreach (var type in specifier)
            {
                _protoMan.TryIndex<DamageGroupPrototype>(type, out var dmgGroup);
                _protoMan.TryIndex<DamageTypePrototype>(type, out var dmgType);
                if (dmgGroup != null)
                {
                    _damageSpec += new DamageSpecifier(dmgGroup, damage);
                }

                if (dmgType != null)
                {
                    _damageSpec += new DamageSpecifier(dmgType, damage);
                }
            }
        }
        */

        _damageSpec = CreateDamageSpecifier(damage, specifier);

        if (_nanitesSystem.TryTakeNanites(uid, damage, component))
        {
            RegenerateHealthByNanites(uid, user, _damageSpec);
            return true;
        }

        return false;
    }

    private DamageSpecifier CreateDamageSpecifier(float damage, string[]? specifier)
    {
        if (specifier == null)
        {
            return new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Brute"), damage);
        }
        if (specifier == default)
        {
            var damageSpec = new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Brute"), damage / 2);
            damageSpec += new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Physical"), damage / 2);
            return damageSpec;
        }

        var damageSpecifier = new DamageSpecifier();
        foreach (var type in specifier)
        {
            if (_protoMan.TryIndex<DamageGroupPrototype>(type, out var dmgGroup))
            {
                damageSpecifier += new DamageSpecifier(dmgGroup, damage);
            }
            if (_protoMan.TryIndex<DamageTypePrototype>(type, out var dmgType))
            {
                damageSpecifier += new DamageSpecifier(dmgType, damage);
            }
        }
        return damageSpecifier;
    }
}

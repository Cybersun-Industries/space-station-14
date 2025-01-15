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
using Content.Server.Radium.Nanites.Systems;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Weapons.Melee.Events;
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
    [Dependency] private readonly NanitesSystem _fuck = default!; // I forgot to add separation in SHARED system and SERVER system

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanitesComponent, MeleeHitEvent>(ApplyNanitesResists);
    }

    private DamageSpecifier? _damageSpec;

    private void RegenerateHealthByNanites(EntityUid uid, DamageSpecifier dmgTypes)
    {
            _damage.TryChangeDamage(uid, dmgTypes);
    }

    // I just deleted "EntityUid user" from this.
    // let's hope I didnt mess this up.
    // after careful consideration this is a real unused variable.
    // dunno what i was thinking back in the days
    /// <summary>
    ///     Name speaks for itself. Currently damage == heal == nanites consumption.
    ///     Probably will change it in a future. Totally not foreshadowing.
    /// </summary>
    public bool TryRegenerateHealthByNanites(EntityUid uid, NanitesComponent component, float damage, string[]? specifier)
    {
        var damageable = EnsureComp<DamageableComponent>(uid);
        if (uid == null || component == null || damageable == null)
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
            RegenerateHealthByNanites(uid, _damageSpec);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Depending on specifier, it returns damage types.
    ///     if specifier is an array then parse through protoman
    ///     and index all damage groups/types
    /// </summary>
    public DamageSpecifier CreateDamageSpecifier(float damage, string[]? specifier)
    {
        // so i missed a thing that we're actually regenerating health. so damage should be < 0
        // so we should invert damage. I hope this wont fuck things up

        var healing = -damage;
        if (specifier == null)
        {
            return new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Brute"), healing);
        }
        if (specifier == default)
        {
            var damageSpec = new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Brute"), healing / 2);
            damageSpec += new DamageSpecifier(_protoMan.Index<DamageGroupPrototype>("Physical"), healing / 2);
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

    private DamageModifierSet? _damageModifierSet;
    public bool SetNanitesResists(EntityUid uid, NanitesComponent component)
    {
        // TODO: I left it. I spent 5 hours doing this and I left it. Now after all this time it's your turn to figure out what i was thinking at this time. GL!
        return true;
    }
    public void ApplyNanitesResists(EntityUid uid, NanitesComponent component, MeleeHitEvent args)
    {
        var activeNanites = _fuck.GetActiveNanites(uid, component);
        if (activeNanites == null)
            return;

    }

}

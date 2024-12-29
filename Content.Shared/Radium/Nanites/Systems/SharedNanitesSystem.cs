using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.CombatMode;
using Content.Shared.Radium.Nanites.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;


namespace Content.Shared.Radium.Nanites.Systems;

public abstract class SharedNanitesSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly MetaDataSystem _metadata = default!;

    [ValidatePrototypeId<AlertPrototype>]
    private const string NanitesAlert = "NanitesAlert";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanitesComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<NanitesComponent, ComponentShutdown>(OnShutdown);

        SubscribeLocalEvent<NanitesComponent, AfterAutoHandleStateEvent>(OnNanitesHandleState);
    }

    private void OnNanitesHandleState(EntityUid uid, NanitesComponent component, ref AfterAutoHandleStateEvent args)
    {
        if (component.Critical)
            EnterNanitesCooldown(uid, component); // so you go 10 second cooldown if you use all of your nanites
        else
        {
            if (component.NanitesDamage > 0f)
                EnsureComp<ActiveNanitesComponent>(uid);

            ExitNanitesCooldown(uid, component);
        }
    }

    private void OnShutdown(EntityUid uid, NanitesComponent component, ComponentShutdown args)
    {
        if (MetaData(uid).EntityLifeStage < EntityLifeStage.Terminating)
        {
            RemCompDeferred<ActiveNanitesComponent>(uid);
        }
        _alerts.ClearAlert(uid, component.NanitesAlert);
    }

    private void OnStartup(EntityUid uid, NanitesComponent component, ComponentStartup args)
    {
        SetNanitesAlert(uid, component);
    }

    private void SetNanitesAlert(EntityUid uid, NanitesComponent? component = null)
    {
        if (!Resolve(uid, ref component, false) || component.Deleted)
            return;

        var severity = ContentHelpers.RoundToLevels(MathF.Max(0f, component.NanitesLevel - component.PowerLevelMax), component.PowerLevelMax, 7);
        _alerts.ShowAlert(uid, component.NanitesAlert, (short) severity);
    }

    public bool TryTakeNanites(EntityUid uid, float takenNanites, NanitesComponent? component = null, EntityUid? source = null, EntityUid? with = null)
    {
        // Something that has no Nanites component automatically passes nanites checks
        if (!Resolve(uid, ref component, false))
            return true;

        if (uid == null || takenNanites == null || takenNanites == 0 || component == null)
            return false;

        var oldLevel = component.NanitesLevel;

        // but should we do this???
        if (oldLevel - takenNanites < component.PowerLevelMin)
            return false;

        TakeNanites(uid, takenNanites, component, oldLevel);
        return true;
    }

    public void TakeNanites(EntityUid uid, float takenNanites, NanitesComponent component, float oldLevel)
    {
        var newLevel = oldLevel - takenNanites;
        component.NanitesLevel = newLevel; // always use TryTakeNanites so doesnt mess up!
    }

    public bool TryAddPowerLevel(EntityUid uid, float amount)
    {
        // Check if the entity has a Nanites component
        if (!TryComp<NanitesComponent>(uid, out _))
            return false;

        // Set the new power level
        AddPowerLevel(uid, amount);

        return true;
    }

    /// <summary>
    ///     Adds to the power level for nanites.
    /// </summary>
    public void AddPowerLevel(EntityUid uid, float amount)
    {
        // Get Nanites component
        if (!TryComp<NanitesComponent>(uid, out var component))
            return;

        // Get new power level
        var newPowerLevel = component.NanitesLevel + amount;

        // Clamp power level using clamp function
        newPowerLevel = Math.Clamp(newPowerLevel, component.PowerLevelMin, component.PowerLevelMax);

        // Set the new power level
        SetPowerLevel(uid, newPowerLevel);
    }


    /// <summary>
    ///     Sets the nanites power level.
    /// </summary>
    public void SetPowerLevel(EntityUid uid, float newPowerLevel)
    {
        // Get nanites component
        if (!TryComp<NanitesComponent>(uid, out var component))
            return;

        // Clamp power level using clamp function
        newPowerLevel = Math.Clamp(newPowerLevel, component.PowerLevelMin, component.PowerLevelMax);

        // Set the new power level
        component.NanitesLvl = newPowerLevel;
    }

    public void UpdateAlert(EntityUid uid, bool enabled, float? powerLevel = null)
    {
        if (!enabled || powerLevel == null)
        {
            _alerts.ClearAlert(uid, NanitesAlert);
            return;
        }

        // Get nanites component
        if (!TryComp<NanitesComponent>(uid, out var component))
            return;

        // 250 / 7 ~= 35
        // Pwr / 35 ~= (0-7)
        // Round to ensure (0-7)
        var power = Math.Clamp(Math.Round(component.NanitesLevel / 35), 0, 7);

        // Set the alert level
        _alerts.ShowAlert(uid, NanitesAlert, (short) power);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        var stamQuery = GetEntityQuery<NanitesComponent>();
        var query = EntityQueryEnumerator<ActiveNanitesComponent>();
        var curTime = _timing.CurTime;

        while (query.MoveNext(out var uid, out _))
        {
            // I'll leave this code here just in case
            if (!stamQuery.TryGetComponent(uid, out var comp) ||
                comp.NanitesDamage <= 0f && !comp.Critical)
            {
                continue;
            }
            Dirty(uid, comp);
        }
    }

    private void EnterNanitesCooldown(EntityUid uid, NanitesComponent? component = null)
    {
        if (!Resolve(uid, ref component) || component.Critical)
            return;

        component.Critical = true;
        component.NanitesDamage = component.PowerLevelMax;

        EnsureComp<ActiveNanitesComponent>(uid);
        Dirty(uid, component);
    }

    private void ExitNanitesCooldown(EntityUid uid, NanitesComponent? component = null)
    {
        if (!Resolve(uid, ref component) || !component.Critical)
            return;

        component.Critical = false;
        component.NanitesDamage = 0f;
        SetNanitesAlert(uid, component);
        RemComp<ActiveNanitesComponent>(uid);
        Dirty(uid, component);
    }

    public bool TryUpdatePowerLevel(EntityUid uid, float frameTime)
    {
        // Check if the entity has a nanites component
        if (!TryComp<NanitesComponent>(uid, out var component))
            return false;

        // Check if power gain is enabled
        if (!component.PowerLevelGainEnabled)
            return false;

        // Set the new power level
        UpdatePowerLevel(uid, frameTime, component);

        return true;
    }

    public void UpdatePowerLevel(EntityUid uid, float frameTime, NanitesComponent component)
    {

        // Calculating new power lowel.
        var newPowerLevel = component.NanitesLvl + frameTime * component.PowerLevelGain * component.PowerLevelGainMultiplier;

        // Clamp power level using clamp function
        newPowerLevel = Math.Clamp(newPowerLevel, component.PowerLevelMin, component.PowerLevelMax);

        // Set the new power level
        SetPowerLevel(uid, newPowerLevel);
    }

}

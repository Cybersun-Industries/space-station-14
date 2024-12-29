using Content.Shared.Radium.Nanites.Systems;
using Content.Shared.Radio;
using Content.Shared.Radium.Nanites;
using Robust.Shared.Prototypes;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Radium.Nanites.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true), AutoGenerateComponentPause]
public sealed partial class NanitesComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public bool Critical;

    // [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    // public float CritThreshold = 100f;

    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float NanitesDamage; // actually i dont even remember what this is

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public TimeSpan NanitesCooldown = TimeSpan.FromSeconds(10);

    [DataField]
    public ProtoId<AlertPrototype> NanitesAlert = "Nanites";

    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float NanitesLevel
    {
        get => NanitesLvl;
        set => NanitesLvl = Math.Clamp(value, PowerLevelMin, PowerLevelMax);
    }
    public float NanitesLvl = 150f;

    /// <summary>
    ///     Don't let PowerLevel go above this value.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float PowerLevelMax = PowerThresholds[NanitesThreshold.Max];

    /// <summary>
    ///     Dont let PowerLevel go below this value.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public float PowerLevelMin = PowerThresholds[NanitesThreshold.Min];

    /// <summary>
    ///     How much nanites is gained per second.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float PowerLevelGain = 0.75f;

    /// <summary>
    ///     Power gain multiplier
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float PowerLevelGainMultiplier = 1f;

    /// <summary>
    ///     Whether to gain power or not.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public bool PowerLevelGainEnabled = true;


    public static readonly Dictionary<NanitesThreshold, float> PowerThresholds = new()
    {
        { NanitesThreshold.Max, 300.0f },
        { NanitesThreshold.Great, 200.0f },
        { NanitesThreshold.Good, 150.0f },
        { NanitesThreshold.Okay, 100.0f },
        { NanitesThreshold.Tired, 50.0f },
        { NanitesThreshold.Min, 0.0f },
    };


}

public enum NanitesThreshold : byte
    {
        Max = 1 << 4,
        Great = 1 << 3,
        Good = 1 << 2,
        Okay = 1 << 1,
        Tired = 1 << 0,
        Min = 0,
    }

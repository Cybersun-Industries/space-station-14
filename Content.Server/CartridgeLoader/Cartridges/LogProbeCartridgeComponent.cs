using Content.Shared._CorvaxNext.CartridgeLoader.Cartridges;
using Content.Shared.CartridgeLoader.Cartridges;
using Content.Shared._CorvaxNext.CartridgeLoader.Cartridges;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.CartridgeLoader.Cartridges;

[RegisterComponent, Access(typeof(LogProbeCartridgeSystem))]
[AutoGenerateComponentPause]
public sealed partial class LogProbeCartridgeComponent : Component
{
    /// <summary>
    /// The name of the scanned entity, sent to clients when they open the UI.
    /// </summary>
    [DataField]
    public string EntityName = string.Empty;

    /// <summary>
    /// The list of pulled access logs
    /// </summary>
    [DataField, ViewVariables]
    public List<PulledAccessLog> PulledAccessLogs = new();

    /// <summary>
    /// The sound to make when we scan something with access
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier SoundScan = new SoundPathSpecifier("/Audio/Machines/scan_finish.ogg");

    /// <summary>
    /// Corvax-Next-PDAChat: The last scanned NanoChat data, if any
    /// </summary>
    [DataField]
    public NanoChatData? ScannedNanoChatData;

    /// <summary>
    /// Corvax-Next-PDAChat: The last scanned NanoChat data, if any
    /// </summary>
    [DataField]
    public NanoChatData? ScannedNanoChatData;
}

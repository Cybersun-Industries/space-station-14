using Content.Shared.Radium.Nanites.Systems;
using Content.Shared.Radio;
using Content.Shared.Radium.Nanites;
using Robust.Shared.Prototypes;

namespace Content.Shared.Radium.Nanites.Components;

/// <summary>
/// A component for Criminal Record Console storing an active station record key and a currently applied filter
/// </summary>
[RegisterComponent]
[Access(typeof(SharedNanitesConsoleSystem))]
public sealed partial class NCloudControllerConsoleComponent : Component
{
    [DataField]
    public uint? ActiveKey;

    [DataField]
    public ProtoId<RadioChannelPrototype> ScienceChannel = "Science";

    [DataField]
    public uint MaxStringLength = 256;
}

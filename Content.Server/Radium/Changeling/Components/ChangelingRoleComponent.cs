using Content.Shared.Mind;
using Content.Shared.Roles;

namespace Content.Server.Radium.Changeling.Components;

[RegisterComponent]
public sealed partial class ChangelingRoleComponent : BaseMindRoleComponent
{
    public EntityUid? Target { get; set; }
    public EntityUid? Extractions { get; set; }
    public MindComponent? TargetMind { get; set; }
}

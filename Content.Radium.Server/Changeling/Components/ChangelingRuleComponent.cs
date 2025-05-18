using Content.Radium.Server.Changeling.StationEvents;
using Content.Shared.Mind;

namespace Content.Radium.Server.Changeling.Components;

[RegisterComponent][Access(typeof(ChangelingRule), typeof(EntitySystems.ChangelingSystem))]
public sealed partial class ChangelingRuleComponent : Component
{
    public List<(EntityUid mindId, MindComponent mind)> Changelings = [];
}

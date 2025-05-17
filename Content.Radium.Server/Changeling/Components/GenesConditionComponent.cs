using Content.Radium.Server.Changeling.EntitySystems;

namespace Content.Radium.Server.Changeling.Components;

[RegisterComponent, Access(typeof(ChangelingConditionsSystem))]
public sealed partial class GenesConditionComponent : Component
{
    [DataField]
    public int GenesExtracted;
}

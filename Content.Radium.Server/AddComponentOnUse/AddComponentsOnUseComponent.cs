using Robust.Shared.Prototypes;

namespace Content.Server.ADT.AddComponentsOnUse;

[RegisterComponent]
public sealed partial class AddComponentsOnUseComponent : Component
{

    [DataField(required: true)]
    public ComponentRegistry Components = new();

    [DataField]
    public bool DeleteOnUse = true;
}

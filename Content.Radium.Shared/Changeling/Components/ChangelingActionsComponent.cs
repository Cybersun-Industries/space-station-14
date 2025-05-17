using Robust.Shared.GameStates;

namespace Content.Radium.Shared.Changeling.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class ChangelingActionComponent : Component
{
    [DataField("chemicalCost")]
    public float ChemicalCost;

    [DataField("useInLesserForm")]
    public bool UseInLesserForm;

    [DataField("requireAbsorbed")]
    public float RequireAbsorbed;
}

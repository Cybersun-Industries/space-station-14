namespace Content.Shared.Radium.Megafauna.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class AshDrakeComponent : Component
{
    [DataField("actionLavajump")]
    public EntityUid? AshDrakeLavajumpAction = null;
    [DataField("actionMeteorRain")]
    public EntityUid? AshDrakeMeteorRainAction = null;
    [DataField("actionFireCone")]
    public EntityUid? AshDrakeFireConeAction = null;
    [DataField("actionFireLine")]
    public EntityUid? AshDrakeFireLineAction = null;
}

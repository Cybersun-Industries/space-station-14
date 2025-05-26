using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Radium.Common.Stamps;

[RegisterComponent]
public sealed partial class CustomizableStampComponent : Component
{
    [DataField]
    public TimeSpan UseDelay = TimeSpan.FromSeconds(0.5);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan LastUseAttempt;
};

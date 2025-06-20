using JetBrains.Annotations;

namespace Content.Radium.Shared.VoidWalker;

/// <summary>
/// Raised when VoidShifting occurs
/// </summary>
[PublicAPI]
public sealed class VoidShiftingEvent : EntityEventArgs
{
    public bool? Shifted;

    public EntityUid User;

    public VoidShiftingEvent(EntityUid user, bool? shifted)
    {
        User = user;
        Shifted = shifted;
    }
}

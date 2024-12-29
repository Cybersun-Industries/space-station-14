using Robust.Shared.Serialization;

namespace Content.Shared.Radium.Nanites.Events;

[Serializable, NetSerializable]
public sealed class NanitesEvents : EntityEventArgs
{
    // TODO: implement a proper nanites event after you finish doing programs
    public NetEntity Performer { get; }
    public NanitesEvents(NetEntity performer)
    {
        Performer = performer;
    }
}

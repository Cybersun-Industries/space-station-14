using Content.Shared.Radium.Nanites;
using Robust.Shared.Serialization;

namespace Content.Shared.Radium.Nanites;

[Serializable, NetSerializable]
public enum NProgrammerConsoleKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class NProgrammerConsoleState : BoundUserInterfaceState
{


    public Nanites? Nanite = null;
    public readonly Dictionary<uint, string>? NanitesListing;

    public NProgrammerConsoleState(Dictionary<uint, string>? nanitesListing)
    {
        NanitesListing = nanitesListing;
    }


    public NProgrammerConsoleState() : this(null)
    {
    }

    public bool IsEmpty()
    {
        return Nanite == null;
    }
}


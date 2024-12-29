using Content.Shared.Radium.Nanites;
using Robust.Shared.Serialization;

namespace Content.Shared.Radium.Nanites;

[Serializable, NetSerializable]
public enum NChamberControllerConsoleKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class NChamberControllerConsoleState : BoundUserInterfaceState
{


    public Nanites? Nanite = null;
    public readonly Dictionary<uint, string>? NanitesListing;

    public NChamberControllerConsoleState(Dictionary<uint, string>? nanitesListing)
    {
        NanitesListing = nanitesListing;
    }


    public NChamberControllerConsoleState() : this(null)
    {
    }

    public bool IsEmpty()
    {
        return Nanite == null;
    }
}


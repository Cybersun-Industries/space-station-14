using Content.Shared.Radium.Nanites;
using Robust.Shared.Serialization;

namespace Content.Shared.Radium.Nanites;

[Serializable, NetSerializable]
public enum NProgramHubConsoleKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class NProgramHubConsoleState : BoundUserInterfaceState
{


    public Nanites? Nanite = null;
    public readonly Dictionary<uint, string>? NanitesListing;

    public NProgramHubConsoleState(Dictionary<uint, string>? nanitesListing)
    {
        NanitesListing = nanitesListing;
    }


    public NProgramHubConsoleState() : this(null)
    {
    }

    public bool IsEmpty()
    {
        return Nanite == null;
    }
}


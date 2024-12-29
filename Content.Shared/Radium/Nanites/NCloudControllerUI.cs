using Content.Shared.Radium.Nanites;
using Robust.Shared.Serialization;

namespace Content.Shared.Radium.Nanites;

[Serializable, NetSerializable]
public enum NCloudControllerConsoleKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class NCloudControllerConsoleState : BoundUserInterfaceState
{


    public Nanites? Nanite = null;
    public readonly Dictionary<uint, string>? NanitesListing;

    public NCloudControllerConsoleState(Dictionary<uint, string>? nanitesListing)
    {
        NanitesListing = nanitesListing;
    }


    public NCloudControllerConsoleState() : this(null)
    {
    }

    public bool IsEmpty()
    {
        return Nanite == null;
    }
}


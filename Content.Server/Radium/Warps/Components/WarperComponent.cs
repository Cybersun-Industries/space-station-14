using Content.Shared.DragDrop;
using Content.Shared.Radium.Warps;

namespace Content.Server.Radium.Warps.Components;

[RegisterComponent]
public sealed class WarperComponent : SharedWarperComponent
{
    /// Warp destination unique identifier.
    [DataField("id")]
    public string? ID { get; set; }


    public override bool DragDropOn(DragDropEvent eventArgs)
    {
        return true;
    }
}

using Robust.Shared.Serialization;

namespace Content.Radium.Shared.Stamps.UI;

[Serializable, NetSerializable]
public sealed class CustomizableStampBoundUserInterfaceState(Color color, string text) : BoundUserInterfaceState
{
    public Color Color = color;
    public string Text = text;
};

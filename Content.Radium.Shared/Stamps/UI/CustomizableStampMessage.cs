using Robust.Shared.Serialization;

namespace Content.Radium.Shared.Stamps.UI;

[Serializable, NetSerializable]
public sealed class CustomizableStampMessage(Color color, string text) : BoundUserInterfaceMessage
{
    public Color Color = color;
    public string Text = text;
};

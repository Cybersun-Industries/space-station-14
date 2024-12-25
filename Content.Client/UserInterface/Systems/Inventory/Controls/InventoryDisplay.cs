using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public sealed class InventoryDisplay : LayoutContainer
{
    private int _columns;
    private int _rows;
    private const int ButtonSpacing = 5;
    private const int ButtonSize = 75;
    private readonly Control _resizer;

    private readonly Dictionary<string, (SlotControl, Vector2i)> _buttons = new();

    public InventoryDisplay()
    {
        _resizer = new Control();
        AddChild(_resizer);
    }

    public SlotControl AddButton(SlotControl newButton, Vector2i buttonOffset)
    {
        AddChild(newButton);
        HorizontalExpand = true;
        VerticalExpand = true;
        InheritChildMeasure = true;
        if (!_buttons.TryAdd(newButton.SlotName, (newButton, buttonOffset)))
            Logger.GetSawmill("InventoryDisplay").Warning("Tried to add button without a slot!");
        SetPosition(newButton, buttonOffset * ButtonSize + new Vector2(ButtonSpacing, ButtonSpacing));
        UpdateSizeData(buttonOffset);
        return newButton;
    }

    public SlotControl? GetButton(string slotName)
    {
        return !_buttons.TryGetValue(slotName, out var foundButton) ? null : foundButton.Item1;
    }

    private void UpdateSizeData(Vector2i buttonOffset)
    {
        var (x, _) = buttonOffset;
        if (x > _columns)
            _columns = x;
        var (_, y) = buttonOffset;
        if (y > _rows)
            _rows = y;
        _resizer.SetHeight = (_rows + 1) * (ButtonSize + ButtonSpacing);
        _resizer.SetWidth = (_columns + 1) * (ButtonSize + ButtonSpacing);
    }

    public bool TryGetButton(string slotName, out SlotControl? button)
    {
        var success = _buttons.TryGetValue(slotName, out var buttonData);
        button = buttonData.Item1;
        return success;
    }

    public void RemoveButton(string slotName)
    {
        if (!_buttons.Remove(slotName))
            return;
        //recalculate the size of the control when a slot is removed
        _columns = 0;
        _rows = 0;
        foreach (var (_, (_, buttonOffset)) in _buttons)
        {
            UpdateSizeData(buttonOffset);
        }
    }
    public void ClearButtons()
    {
        Children.Clear();
    }
}

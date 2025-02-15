﻿using System.Numerics;
using Content.Client.Stylesheets;
using Content.Shared.Backmen.Blob;
using Content.Shared.Backmen.Blob.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.Backmen.Blob;

[GenerateTypedNameReferences]
public sealed partial class BlobChemSwapMenu : DefaultWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    private readonly SpriteSystem _sprite;
    public event Action<BlobChemType>? OnIdSelected;

    private Dictionary<BlobChemType, Color> _possibleChems = new();
    private BlobChemType _selectedId;

    public BlobChemSwapMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        _sprite = _entityManager.System<SpriteSystem>();

    }

    public void UpdateState(Dictionary<BlobChemType, Color> chemList, BlobChemType selectedChem)
    {
        _possibleChems = chemList;
        _selectedId = selectedChem;
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        ClearGrid();

        var group = new ButtonGroup();

        foreach (var blobChem in _possibleChems)
        {
            if (!_prototypeManager.TryIndex("NormalBlobTile", out EntityPrototype? proto))
                continue;

            var button = new Button
            {
                MinSize = new Vector2(64, 64),
                HorizontalExpand = true,
                Group = group,
                StyleClasses = {StyleBase.ButtonSquare},
                ToggleMode = true,
                Pressed = _selectedId == blobChem.Key,
                ToolTip = Loc.GetString($"blob-chem-{blobChem.Key.ToString().ToLower()}-info"),
                TooltipDelay = 0.01f,
            };
            button.OnPressed += _ => OnIdSelected?.Invoke(blobChem.Key);
            Grid.AddChild(button);

            var texture = _sprite.GetPrototypeIcon(proto);
            button.AddChild(new TextureRect
            {
                Stretch = TextureRect.StretchMode.KeepAspectCentered,
                Modulate = blobChem.Value,
                Texture = texture.Default,
            });
        }
    }

    private void ClearGrid()
    {
        Grid.RemoveAllChildren();
    }
}

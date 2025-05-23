﻿using Content.Client.Gameplay;
using Content.Client.Ghost;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.Body.Part;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client._Shitcode.UserInterface.Systems.Surgery.Widgets.Systems;


public sealed class DamagePartsUiController : UIController, IOnStateEntered<GameplayState>,
    IOnSystemChanged<ClientDamagePartsSystem>
{
    [UISystemDependency] private readonly ClientDamagePartsSystem? _partsSystem = null;
    [UISystemDependency] private readonly GhostSystem? _ghost = null;
    [Dependency] private readonly IPlayerManager _playerManager = null!;
    private DamagePartsUi? UI => UIManager.GetActiveUIWidgetOrNull<DamagePartsUi>();

    public void ClearAllControls(object? sender, EventArgs eventArgs)
    {
        UI?.Clear();
    }

    public override void Initialize()
    {
        base.Initialize();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void OnScreenLoad()
    {
        SyncParts(_playerManager.LocalSession?.AttachedEntity);
    }
    private void SystemOnSyncParts(object? sender, IReadOnlyDictionary<(BodyPartType, BodyPartSymmetry), (int, bool)> e)
    {
        if (sender is not ClientDamagePartsSystem system)
            return;
        if (_ghost is { IsGhost: false })
        {
            UI?.SyncControls(system, e);
        }
    }

    public void OnStateEntered(GameplayState state)
    {
        // initially populate the frame if system is available
        SyncParts(_playerManager.LocalSession?.AttachedEntity);
    }

    public void SyncParts(EntityUid? entityUid)
    {
        var uid = entityUid ?? _playerManager.LocalEntity;
        var parts = _partsSystem?.PartsCondition(uid);
        if (parts != null)
        {
            SystemOnSyncParts(_partsSystem, parts);
        }
    }

    public void OnSystemLoaded(ClientDamagePartsSystem system)
    {
        system.SyncParts += SystemOnSyncParts;
        system.Dispose += ClearAllControls;
    }

    public void OnSystemUnloaded(ClientDamagePartsSystem system)
    {
        system.SyncParts -= SystemOnSyncParts;
        system.Dispose -= ClearAllControls;
    }
}


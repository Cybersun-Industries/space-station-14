﻿using Content.Radium.Shared.Changeling.Components;
using Content.Radium.Shared.Changeling.Events;
using JetBrains.Annotations;

namespace Content.Radium.Client.Changeling.UI;

[UsedImplicitly]
public sealed class ChangelingDnaStorageBoundUserInterfaceTransform(EntityUid owner, Enum uiKey)
    : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private Content.Radium.Client.Changeling.UI.ChangelingStorage? _storageMenu;

    protected override void Open()
    {
        base.Open();
        _storageMenu = new Content.Radium.Client.Changeling.UI.ChangelingStorage(this, Owner);
        _storageMenu?.UpdateIdentities(Owner);
        _storageMenu?.OpenCentered();
        if (_storageMenu != null)
            _storageMenu.OnClose += Close;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _storageMenu?.Close();
        _storageMenu?.Dispose();
    }

    public void ConfirmTransformation(NetEntity uid, int index)
    {
        var ev = new ConfirmTransformation
        {
            Uid = uid,
            ServerIdentityIndex = index,
        };
        SendMessage(ev);
        Dispose();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case ChangelingStorageUiState msg:
                _storageMenu?.UpdateIdentities(Owner);
                break;
        }
    }
}

public sealed class ChangelingDnaStorageBoundUserInterfaceSting(EntityUid owner, Enum uiKey)
    : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private Content.Radium.Client.Changeling.UI.ChangelingStorage? _storageMenu;

    protected override void Open()
    {
        base.Open();
        _storageMenu = new Content.Radium.Client.Changeling.UI.ChangelingStorage(this, Owner);
        _storageMenu?.UpdateIdentities(Owner);
        _storageMenu?.OpenCentered();
        if (_storageMenu != null)
            _storageMenu.OnClose += Close;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _storageMenu?.Close();
        _storageMenu?.Dispose();
    }

    public void ConfirmSting(NetEntity uid, int index)
    {
        var ev = new ConfirmTransformSting
        {
            Uid = uid,
            ServerIdentityIndex = index,
        };

        SendMessage(ev);
        Dispose();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case ChangelingStorageUiState:
                _storageMenu?.UpdateIdentities(Owner);
                break;
        }
    }
}

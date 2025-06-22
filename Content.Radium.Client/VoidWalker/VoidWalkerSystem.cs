using Content.Radium.Shared.VoidWalker;
using Robust.Client.Graphics;
using Robust.Shared.Player;

namespace Content.Radium.Client.VoidWalker;

public sealed class VoidWalkerSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ISharedPlayerManager _playerMan = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;

    private VoidWalkerOverlay _overlay = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoidWalkerComponent, ComponentInit>(OnVoidWalkerInit);
        SubscribeLocalEvent<VoidWalkerComponent, ComponentShutdown>(OnVoidWalkerShutdown);
        SubscribeLocalEvent<VoidWalkerComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<VoidWalkerComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
    }


    private void OnVoidWalkerInit(EntityUid uid, VoidWalkerComponent component, ComponentInit args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        _lightManager.DrawLighting = false;
        _overlayMan.AddOverlay(_overlay);

    }

    private void OnVoidWalkerShutdown(EntityUid uid, VoidWalkerComponent component, ComponentShutdown args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        _lightManager.DrawLighting = true;
        _overlayMan.RemoveOverlay(_overlay);

    }

    private void OnPlayerAttached(EntityUid uid, VoidWalkerComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
        _lightManager.DrawLighting = false;
    }

    private void OnPlayerDetached(EntityUid uid, VoidWalkerComponent component, LocalPlayerDetachedEvent args)
    {
        _overlayMan.RemoveOverlay(_overlay);
        _lightManager.DrawLighting = true;
    }



}
